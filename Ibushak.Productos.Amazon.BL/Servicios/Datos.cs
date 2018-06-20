using Ibushak.Productos.Amazon.BL.Extensiones;
using Ibushak.Productos.Amazon.BL.Model;
using Ibushak.Productos.Core.BL.Adapters;
using Ibushak.Productos.Core.BL.Archivos;
using Ibushak.Productos.Core.DomainModel.Catologos;
using Ibushak.Productos.Core.DomainModel.Datos;
using Ibushak.Productos.Core.DomainModel.Ofertas;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ibushak.Productos.Amazon.BL.Servicios
{
    public class Datos
    {
        public List<ProductoActualizado> lstProductoActualizado { get; private set; }
        private ProductoActualizado productoActualizado { get; set; }
        public bool Cambio { get; private set; } = false;
        public bool CambioProducto { get; private set; } = false;
        public bool cambioDimensiones { get; private set; } = false;
        public bool cambioDimensionesPaquete { get; private set; } = false;
        public bool cambioResumen { get; private set; } = false;
        public bool cambioSimilares { get; private set; } = false;
        public bool cambioCaracteristicas { get; private set; } = false;
        public bool CambioUpcs { get; private set; } = false;
        private Bitacora OBitacora { get; set; }

        public Datos()
        {
            OBitacora = new Bitacora();
        }

        public void ValidacionDatos(Producto producto)
        {
            InicializarVariables(producto);

            OBitacora.GuardarLinea($"{ DateTime.Now :yyyy-MM-dd hh:mm:ss}|Productos|Validando datos producto");
            ValidacionDatosProducto(producto);

            if(producto.Dimensiones != null)
            {
                OBitacora.GuardarLinea($"{ DateTime.Now :yyyy-MM-dd hh:mm:ss}|Productos|Validando datos dimensiones");
                ValidacionDatosDimensiones(producto.Dimensiones);
            }
            
            if(producto.DimensionesPaquete != null)
            {
                OBitacora.GuardarLinea($"{ DateTime.Now :yyyy-MM-dd hh:mm:ss}|Productos|Validando datos dimensiones paquete");
                ValidacionDatosDimensionesPaquete(producto.DimensionesPaquete);
            }
            
            if(producto.Resumen != null)
            {
                OBitacora.GuardarLinea($"{ DateTime.Now :yyyy-MM-dd hh:mm:ss}|Productos|Validando datos resumen");
                ValidacionResumen(producto.Resumen);
            }
            
            if(producto.Similares != null)
            {
                OBitacora.GuardarLinea($"{ DateTime.Now :yyyy-MM-dd hh:mm:ss}|Productos|Validando datos similares");
                ValidacionDatosSimilares(producto.Similares);
            }
            
            if(producto.Caracteristicas != null)
            {
                OBitacora.GuardarLinea($"{ DateTime.Now :yyyy-MM-dd hh:mm:ss}|Productos|Validando datos caracteristicas");
                ValidacionDatosCaracteristicas(producto.Caracteristicas);
            }
            
            if(producto.UPCs != null)
            {
                OBitacora.GuardarLinea($"{ DateTime.Now :yyyy-MM-dd hh:mm:ss}|Productos|Validando datos upcs");
                ValidacionDatosUpCs(producto.UPCs);
            }
            
            AgregarProducto();
        }

        public bool ActualizarDatos(Producto producto)
        {
            OBitacora.GuardarLinea($"{ DateTime.Now :yyyy-MM-dd hh:mm:ss}|Productos|Actualizando datos producto");
            if (CambioProducto)
                ProductosAdapter.Actualizar(producto);

            OBitacora.GuardarLinea($"{ DateTime.Now :yyyy-MM-dd hh:mm:ss}|Productos|Actualizando datos dimensiones");
            if (cambioDimensiones)
                DimensionesAdapter.Actualizar(producto.Dimensiones);

            OBitacora.GuardarLinea($"{ DateTime.Now :yyyy-MM-dd hh:mm:ss}|Productos|Actualizando datos dimensiones paquete");
            if (cambioDimensionesPaquete)
                DimensionesPaqueteAdapter.Actualizar(producto.DimensionesPaquete);

            OBitacora.GuardarLinea($"{ DateTime.Now :yyyy-MM-dd hh:mm:ss}|Productos|Actualizando datos resumen");
            if (cambioResumen)
                ResumenAdapter.Actualizar(producto.Resumen);

            OBitacora.GuardarLinea($"{ DateTime.Now :yyyy-MM-dd hh:mm:ss}|Productos|Actualizando datos simialres");
            if (cambioSimilares)
                SimilaresAdapter.Actualizar(producto.Similares);

            OBitacora.GuardarLinea($"{ DateTime.Now :yyyy-MM-dd hh:mm:ss}|Productos|Actualizando datos caracteristicas");
            if (cambioCaracteristicas)
                CaracteristicasAdapter.Actualizar(producto.Caracteristicas);

            OBitacora.GuardarLinea($"{ DateTime.Now :yyyy-MM-dd hh:mm:ss}|Productos|Actualizando datos upcs");
            if (CambioUpcs)
                UPCsAdapter.Actualizar(producto.UPCs);

            if (!CambioProducto && (cambioDimensiones || cambioDimensionesPaquete || cambioResumen || cambioSimilares || cambioCaracteristicas || CambioUpcs))
                ProductosAdapter.ActualizarBandera(producto.ASIN, true);

            CambioProducto = false;
            cambioDimensiones = false;
            cambioDimensionesPaquete = false;
            cambioResumen = false;
            cambioSimilares = false;
            cambioCaracteristicas = false;
            CambioUpcs = false;

            return true;
        }

        private void InicializarVariables(Producto producto)
        {
            if (lstProductoActualizado == null)
                lstProductoActualizado = new List<ProductoActualizado>();

            productoActualizado = new ProductoActualizado
            {
                Campos = new List<CampoActualizado>(),
                ASIN = producto.ASIN,
                UPC = producto.UPC
            };
        }

        private void AgregarProducto()
        {
            if(CambioProducto || cambioDimensiones || cambioDimensionesPaquete || cambioResumen || cambioSimilares ||cambioCaracteristicas || CambioUpcs)
                lstProductoActualizado.Add(productoActualizado);
        }

        private void ValidacionDatosProducto(Producto productoNuevo)
        {
            Producto productoViejo = ProductosAdapter.ObtenerProducto(productoNuevo.ASIN);
            List<CampoActualizado> lstCamposActualizado = productoNuevo.DetailedCompare(productoViejo);

            if (lstCamposActualizado.Count <= 0) return;
            productoActualizado.Campos.AddRange(lstCamposActualizado);
            CambioProducto = true;
            Cambio = true;
        }

        private void ValidacionDatosDimensiones(Dimensiones dimensionesNuevo)
        {
            Dimensiones dimensionesViejo = DimensionesAdapter.ObtenerDimensiones(dimensionesNuevo.ASIN);
            List<CampoActualizado> lstCamposActualizado = dimensionesNuevo.DetailedCompare(dimensionesViejo);
            if (lstCamposActualizado.Count <= 0) return;
            productoActualizado.Campos.AddRange(lstCamposActualizado);
            cambioDimensiones = true;
            Cambio = true;
        }

        private void ValidacionDatosDimensionesPaquete(DimensionesPaquete dimensionesPaqueteNuevo)
        {
            DimensionesPaquete dimensionesPaqueteViejo = DimensionesPaqueteAdapter.ObtenerDimensionesPaquete(dimensionesPaqueteNuevo.ASIN);
            List<CampoActualizado> lstCamposActualizado = dimensionesPaqueteNuevo.DetailedCompare(dimensionesPaqueteViejo);
            if (lstCamposActualizado.Count <= 0) return;
            productoActualizado.Campos.AddRange(lstCamposActualizado);
            cambioDimensionesPaquete = true;
            Cambio = true;
        }

        private void ValidacionResumen(Resumen resumenNuevo)
        {
            Resumen resumenViejo = ResumenAdapter.ObtenerResumen(resumenNuevo.ASIN);
            List<CampoActualizado> lstCamposActualizado = resumenNuevo.DetailedCompare(resumenViejo);
            if (lstCamposActualizado.Count <= 0) return;
            productoActualizado.Campos.AddRange(lstCamposActualizado);
            cambioResumen = true;
            Cambio = true;
        }

        private void ValidacionDatosSimilares(List<Similares> lstSimilaresNuevos)
        {
            if(lstSimilaresNuevos != null)
            {
                var asin = lstSimilaresNuevos.First().ASIN;
                var lstSimilaresViejos = SimilaresAdapter.ObtenerSimilares(asin);

                var lstAsinSimilaresNuevos = (from similar in lstSimilaresNuevos
                                                       select similar.ASINSimilar).ToList();

                var lstAsinSimilaresViejos = (from similar in lstSimilaresViejos
                                                       select similar.ASINSimilar).ToList();

                var nuevos = lstAsinSimilaresNuevos.Except(lstAsinSimilaresViejos);
                var bajas = lstAsinSimilaresViejos.Except(lstAsinSimilaresNuevos);

                if (nuevos.Any())
                {
                    var lstCamposActualizado = new List<CampoActualizado>();
                    foreach (var nuevo in nuevos)
                    {
                        CampoActualizado campo = new CampoActualizado
                        {
                            Campo = "ASINSimilar",
                            ValorNuevo = nuevo
                        };

                        lstCamposActualizado.Add(campo);
                    }

                    productoActualizado.Campos.AddRange(lstCamposActualizado);
                    cambioSimilares = true;
                    Cambio = true;
                }

                if (bajas.Count() > 0)
                {
                    List<CampoActualizado> lstCamposActualizado = new List<CampoActualizado>();
                    foreach (var baja in bajas)
                    {
                        var campo = new CampoActualizado
                        {
                            Campo = "ASINSimilar",
                            ValorViejo = baja,
                            ValorNuevo = ""
                        };

                        lstCamposActualizado.Add(campo);
                    }

                    productoActualizado.Campos.AddRange(lstCamposActualizado);
                    cambioSimilares = true;
                    Cambio = true;
                }
            }
        }

        private void ValidacionDatosCaracteristicas(List<Caracteristicas> lstCaracteristicasNuevas)
        {
            if(lstCaracteristicasNuevas != null)
            {
                string asin = lstCaracteristicasNuevas.First().ASIN;
                List<Caracteristicas> lstCaracteristicasViejas = CaracteristicasAdapter.ObtenerCaracteristicas(asin);

                List<string> lstCaracteristicasNuevos = (from caracteristica in lstCaracteristicasNuevas
                                                         select caracteristica.Caracteristica).ToList();

                List<string> lstCaracteristicasViejos = (from caracteristica in lstCaracteristicasViejas
                                                         select caracteristica.Caracteristica).ToList();

                var nuevos = lstCaracteristicasNuevos.Except(lstCaracteristicasViejos);
                var bajas = lstCaracteristicasViejos.Except(lstCaracteristicasNuevos);

                if (nuevos.Count() > 0)
                {
                    List<CampoActualizado> lstCamposActualizado = new List<CampoActualizado>();
                    foreach (var nuevo in nuevos)
                    {
                        CampoActualizado campo = new CampoActualizado
                        {
                            Campo = "Caracteristicas",
                            ValorNuevo = nuevo
                        };

                        lstCamposActualizado.Add(campo);
                    }

                    productoActualizado.Campos.AddRange(lstCamposActualizado);

                    cambioCaracteristicas = true;
                    Cambio = true;
                }

                if (bajas.Count() > 0)
                {
                    List<CampoActualizado> lstCamposActualizado = new List<CampoActualizado>();
                    foreach (var baja in bajas)
                    {
                        CampoActualizado campo = new CampoActualizado
                        {
                            Campo = "Caracteristicas",
                            ValorViejo = baja,
                            ValorNuevo = ""
                        };

                        lstCamposActualizado.Add(campo);
                    }

                    productoActualizado.Campos.AddRange(lstCamposActualizado);

                    cambioCaracteristicas = true;
                    Cambio = true;
                }
            }
            
        }

        private void ValidacionDatosUpCs(List<UPCs> lstUpcsNuevos)
        {
            if(lstUpcsNuevos != null)
            {
                string asin = lstUpcsNuevos.First().ASIN;
                List<UPCs> lstUpcsViejos = UPCsAdapter.ObtenerUpcs(asin);

                List<string> lstUpcNuevos = (from upc in lstUpcsNuevos
                                             select upc.UPC).ToList();

                List<string> lstUpcViejos = (from upc in lstUpcsViejos
                                                         select upc.UPC).ToList();

                IEnumerable<string> nuevos = lstUpcNuevos.Except(lstUpcViejos);
                IEnumerable<string> bajas = lstUpcViejos.Except(lstUpcNuevos);

                if (nuevos.Count() > 0)
                {
                    List<CampoActualizado> lstCamposActualizado = new List<CampoActualizado>();
                    foreach (var nuevo in nuevos)
                    {
                        CampoActualizado campo = new CampoActualizado
                        {
                            Campo = "UPCs",
                            ValorNuevo = nuevo
                        };

                        lstCamposActualizado.Add(campo);
                    }

                    productoActualizado.Campos.AddRange(lstCamposActualizado);

                    CambioUpcs = true;
                    Cambio = true;
                }

                if (bajas.Count() > 0)
                {
                    List<CampoActualizado> lstCamposActualizado = new List<CampoActualizado>();
                    foreach (var baja in bajas)
                    {
                        CampoActualizado campo = new CampoActualizado
                        {
                            Campo = "Caracteristicas",
                            ValorViejo = baja,
                            ValorNuevo = ""
                        };

                        lstCamposActualizado.Add(campo);
                    }

                    productoActualizado.Campos.AddRange(lstCamposActualizado);

                    CambioUpcs = true;
                    Cambio = true;
                }
            }
            
        }
    }
}