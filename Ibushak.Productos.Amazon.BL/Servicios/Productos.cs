using Ibushak.Productos.Amazon.BL.Amazon.ECS;
using Ibushak.Productos.Amazon.BL.Parseo;
using Ibushak.Productos.Amazon.BL.Webservice.AWS;
using Ibushak.Productos.Core.BL.Adapters;
using Ibushak.Productos.Core.BL.Archivos;
using Ibushak.Productos.Core.DomainModel.Catologos;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading;

namespace Ibushak.Productos.Amazon.BL.Servicios
{
    public class Productos
    {
        private AWSHelper OAwsHelper { get; set; }
        private Bitacora OBitacora { get; set; }

        public Productos()
        {
            OBitacora = new Bitacora();
        }

        public void ProcesoProductos()
        {
            OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Inicio de Proceso");
            OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Instanciando Helper AWS");
            OAwsHelper = new AWSHelper("AKIAIHQ53NFR6EGNA75Q", "MKph/aGOqSBvIT+VtCHpR0NHYgs1DEvmBJY4Opym", "ibushak03-20", "AKIAIHQ53NFR6EGNA75Q");
            try
            {
                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Obteniendo productos a analizar");
                var productos = ProductosAdapter.ObtenerProductos();
                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Productos asin en la base actual");
                foreach (var item in productos.asins)
                    OBitacora.GuardarLinea(item.Id);
                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Productos upc en la base actual");
                foreach (var item in productos.upcs)
                    OBitacora.GuardarLinea(item.Id);
                if (productos.upcs.Any())
                    ProcesarUpc(productos.upcs);

                if (productos.asins.Any())
                    ProcesarAsin(productos.asins);

                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Generando Archivo Excel");
                var oArchvios = new Archivos();
                oArchvios.GenerarProductosActualizados();

                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Reportando a NetSuite");
                var changeProds = ProductosAdapter.ObtenerProductosTodos().Where(x => x.IsUpdated);
                var oBuyBox = new BuyBox();
                foreach (var item in changeProds)
                {
                    try
                    {
                        oBuyBox.ReportarNetSuiteInicial(item.BuyBox, item.ASIN);
                        oBuyBox.ReportarNetSuiteComplete(item.ASIN);
                        item.IsUpdated = false;
                        ProductosAdapter.Actualizar(item);
                    }
                    catch (Exception ex)
                    {
                        OBitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{GetMessageError(ex)}/{ex.StackTrace}");
                    }
                }

                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Fin de Proceso");
            }
            catch (Exception ex)
            {
                OBitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{GetMessageError(ex)}/{ex.StackTrace}");
            }
        }

        private void ProcesarUpc(IEnumerable<UPC> lstUpcs)
        {
            ItemLookupResponse responseUpc;

            OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Generando la lista de UPC");
            var lstUpc = lstUpcs.Select(upc => upc.Id).ToList();

            OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Dividiendo la lista de UPC en sublistas de 10");
            var subLstUpc = lstUpc
                            .Select((x, i) => new { Index = i, Value = x })
                            .GroupBy(x => x.Index / 10)
                            .Select(x => x.Select(v => v.Value).ToList())
                            .ToList();

            subLstUpc.ForEach(lst =>
            {
                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Obteniendo datos de AWS de la lista UPC");
                Thread.Sleep(2000);
                responseUpc = OAwsHelper.ItemLookUp(lst, ItemLookupRequestIdType.UPC);

                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Validando Productos UPC");
                ValidacionProductosUpc(responseUpc);
            });
        }

        private void ProcesarAsin(IEnumerable<ASIN> lstAsins)
        {
            ItemLookupResponse responseAsin;

            OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Generando la lista de ASIN");
            var lstAsin = (from asin in lstAsins select asin.Id).ToList();

            OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Dividiendo la lista de ASIN en sublistas de 10");
            var subLstAsin = lstAsin
                            .Select((x, i) => new { Index = i, Value = x })
                            .GroupBy(x => x.Index / 10)
                            .Select(x => x.Select(v => v.Value.Replace("\r", "").Replace("\n", "")).ToList())
                            .ToList();

            subLstAsin.ForEach(lst =>
            {
                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Obteniendo datos de AWS de la lista ASIN");
                Thread.Sleep(2000);

                responseAsin = OAwsHelper.ItemLookUp(lst, ItemLookupRequestIdType.ASIN);
                if (responseAsin.Items.First().Item == null || responseAsin.Items.First().Item.Length != lst.Count)
                {
                    foreach (var item in lst)
                    {
                        if (responseAsin.Items.First().Item == null)
                        {
                            if (!ProductosAdapter.Existe(item))
                            {
                                ProductosAdapter.Insertar(new Producto
                                {
                                    ASIN = item.Replace("\r\n", ""),
                                    Offers = "No esta disponible en la api"
                                });
                            }
                            else
                            {
                                ProductosAdapter.Actualizar(new Producto
                                {
                                    ASIN = item,
                                    Offers = "No esta disponible en la api"
                                });
                            }
                            continue;
                        }
                        var response = responseAsin.Items.First().Item.Select(x => x.ASIN).ToList();
                        if (!response.Contains(item.Replace("\r\n", "")))
                        {
                            if (!ProductosAdapter.Existe(item.Replace("\r\n", "")))
                            {
                                ProductosAdapter.Insertar(new Producto
                                {
                                    ASIN = item.Replace("\r\n", ""),
                                    Offers = "No esta disponible en la api"
                                });
                            }
                            else
                            {
                                ProductosAdapter.Actualizar(new Producto
                                {
                                    ASIN = item.Replace("\r\n", ""),
                                    Offers = "No esta disponible en la api"
                                });
                            }
                        }
                    }
                }

                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Validando Productos ASIN");
                ValidacionProductosAsin(responseAsin);
            });

        }

        private void ValidacionProductosUpc(ItemLookupResponse responseUpc)
        {
            try
            {
                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Revisando que la lista UPC no tenga errores");
                if (responseUpc.Items.First().Request.Errors != null)
                {
                    foreach (var error in responseUpc.Items.First().Request.Errors)
                    {
                        OBitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|{error.Message}|");
                        if (!ProductosAdapter.Existe(error.Message.Split(' ').First()))
                        {
                            try
                            {
                                ProductosAdapter.Insertar(new Producto
                                {
                                    ASIN = error.Message.Split(' ').First(),
                                    Offers = $"{error.Code}|{error.Message}",
                                    UPC = "",
                                    Label = "",
                                    Actualizacion = false,
                                    Amount = "",
                                    Binding = "",
                                    Brand = "",
                                    BuyBox = null,
                                    Caracteristicas = null,
                                    ClothingSize = "",
                                    Color = "",
                                    Comentarios = null,
                                    CurrencyCode = "",
                                    Department = "",
                                    Dimensiones = null,
                                    DimensionesPaquete = null,
                                    EAN = "",
                                    FormattedPrice = "",
                                    LargeImage = "",
                                    LegalDisclaimer = "",
                                    MPN = "",
                                    Manufacture = "",
                                    MediumImage = "",
                                    Model = "",
                                    NumberItems = 0,
                                    PackageQuantity = 0,
                                    PartNumber = "",
                                    ProdcutTypeName = "",
                                    ProductGroup = "",
                                    Publisher = "",
                                    ReleaseDate = "",
                                    Resumen = null,
                                    SalesRank = 0,
                                    Similares = null,
                                    Size = "",
                                    SmallImage = "",
                                    Studio = "",
                                    Title = "",
                                    UPCs = null,
                                    isAdultProduct = false,
                                    isAutographed = false,
                                    isMemorabilia = false
                                });
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error guardar errores amazon: " + GetMessageError(ex));
                                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Guardar amazon error|{ GetMessageError(ex) }");
                            }
                        }
                    }
                }
                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Obteniendo elementos UPC");
                var productosUpc = responseUpc.Items.First();

                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Procesando elementos UPC");
                ProcesarItems(productosUpc);
                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Fin proceso de elementos UPC");
            }
            catch (Exception ex)
            {
                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error Productos|{GetMessageError(ex)}/{ex.StackTrace}");
            }
        }

        private void ValidacionProductosAsin(ItemLookupResponse reponseAsin)
        {
            try
            {
                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Revisando que la lista ASIN no tenga errores");
                if (reponseAsin.Items.First().Request.Errors != null)
                {
                    foreach (var error in reponseAsin.Items.First().Request.Errors)
                    {
                        if (error.Message !=
                            "Este artículo no es accesible mediante la API para publicidad de productos.")
                        {
                            OBitacora.GuardarLinea(
                                $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error respuesta amazon producto|{error.Message}|");
                            if (!ProductosAdapter.Existe(error.Message.Split(' ').First()))
                            {
                                try
                                {
                                    if (ProductosAdapter.Existe(error.Message.Replace("\n", "").Split(' ').First()))
                                    {
                                        ProductosAdapter.Actualizar(new Producto
                                        {
                                            ASIN = error.Message.Replace("\n", "").Split(' ').First(),
                                            Offers = $"{error.Code}|{error.Message}",
                                            UPC = "",
                                            Label = "",
                                            Actualizacion = false,
                                            Amount = "",
                                            Binding = "",
                                            Brand = "",
                                            BuyBox = null,
                                            Caracteristicas = null,
                                            ClothingSize = "",
                                            Color = "",
                                            Comentarios = null,
                                            CurrencyCode = "",
                                            Department = "",
                                            Dimensiones = null,
                                            DimensionesPaquete = null,
                                            EAN = "",
                                            FormattedPrice = "",
                                            LargeImage = "",
                                            LegalDisclaimer = "",
                                            MPN = "",
                                            Manufacture = "",
                                            MediumImage = "",
                                            Model = "",
                                            NumberItems = 0,
                                            PackageQuantity = 0,
                                            PartNumber = "",
                                            ProdcutTypeName = "",
                                            ProductGroup = "",
                                            Publisher = "",
                                            ReleaseDate = "",
                                            Resumen = null,
                                            SalesRank = 0,
                                            Similares = null,
                                            Size = "",
                                            SmallImage = "",
                                            Studio = "",
                                            Title = "",
                                            UPCs = null,
                                            isAdultProduct = false,
                                            isAutographed = false,
                                            isMemorabilia = false
                                        });
                                    }
                                    else
                                    {
                                        ProductosAdapter.Insertar(new Producto
                                        {
                                            ASIN = error.Message.Replace("\n", "").Split(' ').First(),
                                            Offers = $"{error.Code}|{error.Message}",
                                            UPC = "",
                                            Label = "",
                                            Actualizacion = false,
                                            Amount = "",
                                            Binding = "",
                                            Brand = "",
                                            BuyBox = null,
                                            Caracteristicas = null,
                                            ClothingSize = "",
                                            Color = "",
                                            Comentarios = null,
                                            CurrencyCode = "",
                                            Department = "",
                                            Dimensiones = null,
                                            DimensionesPaquete = null,
                                            EAN = "",
                                            FormattedPrice = "",
                                            LargeImage = "",
                                            LegalDisclaimer = "",
                                            MPN = "",
                                            Manufacture = "",
                                            MediumImage = "",
                                            Model = "",
                                            NumberItems = 0,
                                            PackageQuantity = 0,
                                            PartNumber = "",
                                            ProdcutTypeName = "",
                                            ProductGroup = "",
                                            Publisher = "",
                                            ReleaseDate = "",
                                            Resumen = null,
                                            SalesRank = 0,
                                            Similares = null,
                                            Size = "",
                                            SmallImage = "",
                                            Studio = "",
                                            Title = "",
                                            UPCs = null,
                                            isAdultProduct = false,
                                            isAutographed = false,
                                            isMemorabilia = false
                                        });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Error guardar errores amazon: " + GetMessageError(ex));
                                    OBitacora.GuardarLinea(
                                        $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Guardar amazon error|{GetMessageError(ex)}");
                                }
                            }
                        }
                    }
                }
                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Obteniendo elemento ASIN");
                var productosAsin = reponseAsin.Items.First();

                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Procesando elementos ASIN");
                ProcesarItems(productosAsin);
                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Fin de proceso de elementos ASIN");
            }
            catch (DbEntityValidationException ex)
            {
                var resultErrors = ex.EntityValidationErrors.Aggregate("",
                    (current1, validationErrors) => validationErrors.ValidationErrors.Aggregate(current1,
                        (current, validationError) =>
                            current + $"NValid.{validationErrors.Entry.Entity}:{validationError.ErrorMessage}"));
                OBitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{resultErrors} /{GetMessageError(ex)}|");
            }
            catch (Exception ex)
            {
                OBitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{GetMessageError(ex)}/{ex.StackTrace}|");
            }
        }

        private void ProcesarItems(Items items)
        {
            var oItemLookupParseo = new ItemLookupParseo();
            var oBuyBox = new BuyBox();
            var oDatos = new Datos();
            try
            {
                if (items.Item == null) return;
                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Comienza iteracion de elementos");
                foreach (var producto in items.Item)
                {
                    if (producto.Errors != null)
                    {
                        foreach (var error in producto.Errors)
                        {
                            OBitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error respuesta amazon producto|{error.Message}|");
                            try
                            {
                                if (ProductosAdapter.Existe(error.Message.Split(' ').First()))
                                {
                                    ProductosAdapter.Actualizar(new Producto
                                    {
                                        ASIN = error.Message.Split(' ').First(),
                                        Offers = $"{error.Code}|{error.Message}"
                                    });
                                }
                                else
                                {
                                    ProductosAdapter.Insertar(new Producto
                                    {
                                        ASIN = error.Message.Split(' ').First(),
                                        Offers = $"{error.Code}|{error.Message}",
                                        UPC = "",
                                        Label = "",
                                        Actualizacion = false,
                                        Amount = "",
                                        Binding = "",
                                        Brand = "",
                                        BuyBox = null,
                                        Caracteristicas = null,
                                        ClothingSize = "",
                                        Color = "",
                                        Comentarios = null,
                                        CurrencyCode = "",
                                        Department = "",
                                        Dimensiones = null,
                                        DimensionesPaquete = null,
                                        EAN = "",
                                        FormattedPrice = "",
                                        LargeImage = "",
                                        LegalDisclaimer = "",
                                        MPN = "",
                                        Manufacture = "",
                                        MediumImage = "",
                                        Model = "",
                                        NumberItems = 0,
                                        PackageQuantity = 0,
                                        PartNumber = "",
                                        ProdcutTypeName = "",
                                        ProductGroup = "",
                                        Publisher = "",
                                        ReleaseDate = "",
                                        Resumen = null,
                                        SalesRank = 0,
                                        Similares = null,
                                        Size = "",
                                        SmallImage = "",
                                        Studio = "",
                                        Title = "",
                                        UPCs = null,
                                        isAdultProduct = false,
                                        isAutographed = false,
                                        isMemorabilia = false
                                    });
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error guardar errores amazon: " + GetMessageError(ex));
                                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Guardar amazon error|{ GetMessageError(ex) }");
                            }
                        }
                    }
                    try
                    {
                        OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Parseando item a producto { producto.ASIN }");
                        var prod = oItemLookupParseo.Parser(producto);
                        OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Validando si existe el producto");
                        if (!ProductosAdapter.Existe(prod.ASIN))
                        {
                            OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Insertando producto a la base de datos");
                            ProductosAdapter.Insertar(prod);
                            oBuyBox.ReportarNetSuiteInicial(prod.BuyBox, prod.ASIN);
                        }
                        else
                        {
                            OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Validando datos");
                            oDatos.ValidacionDatos(prod);

                            OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Actualizando datos");
                            oDatos.ActualizarDatos(prod);

                            OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Validando BuyBox");
                            oBuyBox.ValidarCambios(prod.BuyBox);

                            OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Actualizando BuyBox");
                            oBuyBox.ActualizarBuyBox(prod.BuyBox);
                        }
                        if (oBuyBox.Cambio)
                        {
                            prod.IsUpdated = true;
                            ProductosAdapter.Actualizar(prod);
                            //OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Reportando a NetSuite");
                            //oBuyBox.ReportarNetSuiteComplete(prod.ASIN);
                        }
                    }
                    catch (DbEntityValidationException ex)
                    {
                        var resultErrors = ex.EntityValidationErrors.Aggregate("",
                            (current1, validationErrors) => validationErrors.ValidationErrors.Aggregate(current1,
                                (current, validationError) =>
                                    current + $"NValid.{validationErrors.Entry.Entity}:{validationError.ErrorMessage}"));
                        OBitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{producto.ASIN} {resultErrors} /{GetMessageError(ex)}|");
                    }
                    catch (Exception ex)
                    {
                        OBitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{producto.ASIN} {GetMessageError(ex)}/{ex.StackTrace}|");
                    }
                }
                //OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Reportando a NetSuite");
                //oBuyBox.ReportarNetSuiteComplete();
                //if (oBuyBox.cambio)
                //{
                //    OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Reportando a NetSuite");
                //    //oBuyBox.ReportarNetSuite();
                //    oBuyBox.ReportarNetSuiteComplete(ProductosAdapter.);
                //}
                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Termina iteracion de elementos");
            }
            catch (DbEntityValidationException ex)
            {
                var resultErrors = ex.EntityValidationErrors.Aggregate("",
                    (current1, validationErrors) => validationErrors.ValidationErrors.Aggregate(current1,
                        (current, validationError) =>
                            current + $"NValid.{validationErrors.Entry.Entity}:{validationError.ErrorMessage}"));
                OBitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{resultErrors} /{GetMessageError(ex)}|");
            }
            catch (Exception ex)
            {
                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{GetMessageError(ex)}/{ex.StackTrace}");
            }
        }

        private string GetMessageError(Exception ex)
        {
            var result = ex.Message;
            var temp = ex;
            while (temp.InnerException != null)
            {
                temp = temp.InnerException;
                result += $"|{temp.Message}/{temp.Message}|";
            }
            return result;
        }
    }
}