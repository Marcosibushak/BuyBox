using Ibushak.Productos.Amazon.BL.Extensiones;
using Ibushak.Productos.Amazon.BL.Model;
using Ibushak.Productos.Amazon.BL.Webservice.SuiteTalk;
using Ibushak.Productos.Core.BL.Adapters;
using Ibushak.Productos.Core.BL.Archivos;
using System;
using System.Collections.Generic;
using System.Linq;
using of = Ibushak.Productos.Core.DomainModel.Ofertas;

namespace Ibushak.Productos.Amazon.BL.Servicios
{
    public class BuyBox
    {
        public List<ProductoActualizado> LstProductoActualizado { get; private set; }
        public bool Cambio { get; private set; }
        private bool CambioBuyBox { get; set; }
        private bool CambioMerchant { get; set; }
        private bool EsIbushak { get; set; }
        private Bitacora OBitacora { get; set; }
        private Dictionary<string, Dictionary<string, string>> DicDatosNetSuite { get; set; }

        private const string Box = "custitem_buy_box";
        private const string Precio = "custitem_price_buy_box";

        //public void validarCambiosUPC(List<of.BuyBox> buyBoxes, string upc)
        //{
        //    if (buyBoxes != null)
        //    {
        //        ProductoActualizado productoActualizado = new ProductoActualizado();
        //        productoActualizado.ASIN = buyBoxes.First().ASIN;
        //        productoActualizado.BuyBox = new List<BuyBoxActualizado>();
        //        dicDatosNetSuite = new Dictionary<string, Dictionary<string, string>>();

        //        oBitacora.guardarLinea($"{ DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") }|Productos|Validando datos BuyBox");
        //        foreach (var buyBoxNuevo in buyBoxes)
        //        {
        //            var buyBoxViejo = BuyBoxAdapter.obtenerBuyBox(buyBoxNuevo.ASIN, buyBoxNuevo.Condition);
        //            validarMerchantBuyBox(buyBoxNuevo.Merchant, buyBoxViejo.Merchant);

        //            List<CampoActualizado> lstCamposActualizado = buyBoxNuevo.DetailedCompare(buyBoxViejo);

        //            if (lstCamposActualizado.Count > 0)
        //            {
        //                productoActualizado.BuyBox.AddRange((from campo in lstCamposActualizado
        //                                                     select new BuyBoxActualizado
        //                                                     {
        //                                                         Campo = campo.Campo,
        //                                                         Condicion = buyBoxNuevo.Condition,
        //                                                         ValorNuevo = campo.ValorNuevo != null ? campo.ValorNuevo : null,
        //                                                         ValorViejo = campo.ValorViejo != null ? campo.ValorViejo : null
        //                                                     }).ToList());
        //                cambioBuyBox = true;
        //                cambio = true;
        //            }

        //            agregarDicNetSuite(buyBoxNuevo, upc);
        //        }

        //        if (cambioBuyBox)
        //        {
        //            if (lstProductoActualizado == null)
        //                lstProductoActualizado = new List<ProductoActualizado>();

        //            lstProductoActualizado.Add(productoActualizado);
        //        }
        //    }
        //}

        public BuyBox()
        {
            OBitacora = new Bitacora();
        }

        public void ValidarCambios(List<of.BuyBox> buyBoxes)
        {
            if (buyBoxes != null)
            {
                var productoActualizado = new ProductoActualizado
                {
                    ASIN = buyBoxes.First().ASIN,
                    BuyBox = new List<BuyBoxActualizado>()
                };

                OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Validando datos BuyBox");
                foreach (var buyBoxNuevo in buyBoxes)
                {
                    var buyBoxViejo = BuyBoxAdapter.ObtenerBuyBox(buyBoxNuevo.ASIN, buyBoxNuevo.Condition);

                    if (buyBoxViejo != null)
                    {
                        ValidarMerchantBuyBox(buyBoxNuevo.Merchant, buyBoxViejo.Merchant);

                        List<CampoActualizado> lstCamposActualizado = buyBoxNuevo.DetailedCompare(buyBoxViejo);

                        if (lstCamposActualizado.Count > 0)
                        {
                            productoActualizado.BuyBox.AddRange((from campo in lstCamposActualizado
                                                                 select new BuyBoxActualizado
                                                                 {
                                                                     Campo = campo.Campo,
                                                                     Condicion = buyBoxNuevo.Condition,
                                                                     ValorNuevo = campo.ValorNuevo,
                                                                     ValorViejo = campo.ValorViejo
                                                                 }).ToList());
                            CambioBuyBox = true;
                            Cambio = true;
                            AgregarDicNetSuite(buyBoxNuevo, buyBoxNuevo.ASIN);
                        }
                    }
                    else
                    {
                        CambioBuyBox = true;
                        Cambio = true;

                        ValidarMerchantBuyBox(buyBoxNuevo.Merchant, "");

                        AgregarDicNetSuite(buyBoxNuevo, buyBoxNuevo.ASIN);
                    }

                }

                if (CambioBuyBox)
                {
                    if (LstProductoActualizado == null)
                        LstProductoActualizado = new List<ProductoActualizado>();

                    LstProductoActualizado.Add(productoActualizado);
                }
            }

        }

        public bool ActualizarBuyBox(List<of.BuyBox> buyBoxes)
        {
            OBitacora.GuardarLinea($"{ DateTime.Now:yyyy-MM-dd hh:mm:ss}|Productos|Actualizando datos BuyBox");
            if (CambioBuyBox)
            {
                BuyBoxAdapter.ActualizarBuyBox(buyBoxes);
                ProductosAdapter.ActualizarBandera(buyBoxes.First().ASIN, true);
            }
            CambioBuyBox = false;
            return true;
        }

        public void ReportarNetSuiteComplete(string asin)
        {
            var oStHelper = new StHelper();
            if (DicDatosNetSuite.Count <= 0) return;
            OBitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Comienza a reporte netsuite {asin}");
            OBitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Reportando general");
            foreach (var dato in DicDatosNetSuite)
            {
                try
                {
                    oStHelper.ActualizarHeaders(dato.Value, asin);
                }
                catch (Exception ex)
                {
                    OBitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{ex.Message}/{ex.StackTrace}");
                }
            }
            oStHelper.ActualizarCamposCompleto(asin);
            OBitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Termina a reporte netsuite {asin}");
        }

        public void ReportarNetSuite()
        {
            if (DicDatosNetSuite.Count <= 0) return;
            var oStHelper = new StHelper();
            foreach (var dato in DicDatosNetSuite)
                oStHelper.ActualizarCampos(dato.Value, dato.Key);
        }

        public void ReportarNetSuiteInicial(List<of.BuyBox> buyBoxes, string asin)
        {
            DicDatosNetSuite = new Dictionary<string, Dictionary<string, string>>();
            if (buyBoxes == null)
                buyBoxes = BuyBoxAdapter.ObtenerBuyBox(asin);
            if (DicDatosNetSuite == null)
                DicDatosNetSuite = new Dictionary<string, Dictionary<string, string>>();
            var prod = ProductosAdapter.ObtenerProducto(asin);
            Dictionary<string, string> campos = new Dictionary<string, string>
            {
                {"custitem_ib_update_buybox_date", DateTime.Now.ToString("dd/MM/yyyy")}
            };
            if (buyBoxes != null && buyBoxes.Any())
            {
                campos.Add(Box, buyBoxes.First().Merchant.Equals("Ibushak") ? "true" : "false");
            }
            campos.Add(Precio, prod.FormattedPrice);
            DicDatosNetSuite.Add(asin, campos);
            Cambio = true;
        }

        //public void ReportarNetSuiteInicial(List<of.BuyBox> buyBoxes, string asin)
        //{
        //    DicDatosNetSuite = new Dictionary<string, Dictionary<string, string>>();
        //    if (buyBoxes == null)
        //        buyBoxes = BuyBoxAdapter.ObtenerBuyBox(asin);
        //    if (buyBoxes != null)
        //    {
        //        if (DicDatosNetSuite == null)
        //            DicDatosNetSuite = new Dictionary<string, Dictionary<string, string>>();
        //        Dictionary<string, string> campos = new Dictionary<string, string>
        //        {
        //            {"custitem_ib_update_buybox_date", DateTime.Now.ToString("dd/MM/yyyy")}
        //        };
        //        buyBoxes.ForEach(bb =>
        //        {
        //            campos.Add(Box, bb.Merchant.Equals("Ibushak") ? "true" : "false");
        //            campos.Add(Precio, bb.FormattedPrice);
        //            DicDatosNetSuite.Add(asin, campos);
        //        });
        //        Cambio = true;
        //    }
        //}

        private void ValidarMerchantBuyBox(string merchantNuevo, string merchantViejo)
        {
            if (merchantNuevo.Equals("Ibushak") && !merchantViejo.Equals("Ibushak"))
            {
                CambioMerchant = true;
                EsIbushak = true;
            }
            else if (merchantNuevo.Equals("Ibushak") && merchantViejo.Equals("Ibushak"))
            {
                CambioMerchant = false;
                EsIbushak = true;
            }
            else if (!merchantNuevo.Equals("Ibushak") && merchantViejo.Equals("Ibushak"))
            {
                CambioMerchant = true;
                EsIbushak = false;
            }
            else
            {
                CambioMerchant = false;
            }
        }

        private void AgregarDicNetSuite(of.BuyBox buyBox, string asin)
        {
            Dictionary<string, string> campos = new Dictionary<string, string>();

            if (CambioMerchant)
            {
                campos.Add(Box, EsIbushak.ToString());
            }

            if (CambioBuyBox)
            {
                campos.Add(Precio, buyBox.FormattedPrice);
            }

            if (campos.Count > 0)
            {
                if (DicDatosNetSuite == null)
                    DicDatosNetSuite = new Dictionary<string, Dictionary<string, string>>();

                DicDatosNetSuite.Add(asin, campos);
            }
        }
    }
}
