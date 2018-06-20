using SuiteTalkWs = Ibushak.Productos.Amazon.BL.com.netsuite.webservices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Ibushak.Productos.Core.BL.Adapters;
using Ibushak.Productos.Core.BL.Archivos;
using Ibushak.Productos.Core.DomainModel.Catologos;
using Ibushak.Productos.Core.DomainModel.Datos;
using Ibushak.Productos.Core.DomainModel.Ofertas;

namespace Ibushak.Productos.Amazon.BL.Webservice.SuiteTalk
{
    public class StHelper
    {
        private readonly SuiteTalkWs.NetSuiteService _client;
        private bool StatusAutenticacion { get; set; }
        private bool StatusNotificacion { get; set; }
        private List<string> LstErrores { get; set; }
        private readonly Bitacora _bitacora;

        private const string Box = "custitem_buy_box";
        private const string Precio = "custitem_price_buy_box";
        private const string Asin = "custitem_upc_alt_5";

        public StHelper()
        {
            _bitacora = new Bitacora();
            _client = new SuiteTalkWs.NetSuiteService { CookieContainer = new CookieContainer() };
            StatusAutenticacion = Autenticar();
        }

        public void ActualizarHeaders(Dictionary<string, string> campos, string asin)
        {
            var custFieldList = new SuiteTalkWs.CustomFieldRef[campos.Count];
            if (campos.Count > 0)
            {
                var cont = 0;
                foreach (var campo in campos)
                {
                    if (campo.Key == Box)
                    {
                        var fieldRef = GenerarBooleanCustomFieldRef(campo.Key, campo.Value);
                        custFieldList[cont] = fieldRef;
                    }
                    else
                    {
                        var fieldRef = GenerarStringCustomFieldRef(campo.Key, campo.Value);
                        custFieldList[cont] = fieldRef;
                    }
                    cont++;
                }
            }
            var item = new SuiteTalkWs.InventoryItem();
            var internalId = ObtenerId(asin);
            if (internalId != null && !internalId.Equals("-1"))
            {
                item.internalId = internalId;
                if (campos.Count > 0)
                    item.customFieldList = custFieldList;
                var response = _client.update(item);
                if (!response.status.isSuccess)
                {
                    LstErrores.Add(ReportarErrores(response.status));
                    StatusNotificacion = false;
                }
                else
                {
                    StatusNotificacion = true;
                }
            }
        }

        public void ActualizarCamposCompleto(string asin)
        {
            var product = ProductosAdapter.ObtenerProducto(asin);
            _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Obteniendo {asin} de netsuite");
            var internalId = ObtenerId(asin);
            if (internalId != null && !internalId.Equals("-1"))
            {
                product.Netsuite = "";
                ProductosAdapter.Actualizar(product);
                _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Reportando Buybox");
                try
                {
                    UpdateBuybox(product, internalId);
                }
                catch (Exception ex)
                {
                    _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{ex.Message}/{ex.StackTrace}");
                }
                _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Reportando Buybox Info");
                try
                {
                    UpdateBuyboxInfo(product.ASIN, internalId);
                }
                catch (Exception ex)
                {
                    _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{ex.Message}/{ex.StackTrace}");
                }
                _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Reportando Resumen ofertas");
                try
                {
                    UpdateResumenOfertas(product, internalId);
                }
                catch (Exception ex)
                {
                    _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{ex.Message}/{ex.StackTrace}");
                }
                _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Reportando Dimensiones");
                try
                {
                    UploadDimensiones(product.ASIN, internalId);
                }
                catch (Exception ex)
                {
                    _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{ex.Message}/{ex.StackTrace}");
                }
                _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Reportando Dimensiones paquete");
                try
                {
                    UploadDimensionesPaquete(product.ASIN, internalId);
                }
                catch (Exception ex)
                {
                    _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{ex.Message}/{ex.StackTrace}");
                }
                _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Reportando Similares");
                try
                {
                    UploadSimi(product.ASIN, internalId);
                }
                catch (Exception ex)
                {
                    _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{ex.Message}/{ex.StackTrace}");
                }
                _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Reportando Caracteristicas");
                try
                {
                    UploadCaracteristicas(product.ASIN, internalId);
                }
                catch (Exception ex)
                {
                    _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{ex.Message}/{ex.StackTrace}");
                }
            }
            else
            {
                _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Reportando Buybox");
                try
                {
                    UpdateBuybox(product, "");
                }
                catch (Exception ex)
                {
                    _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{ex.Message}/{ex.StackTrace}");
                }
                _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Reportando Buybox Info");
                try
                {
                    UpdateBuyboxInfo(product.ASIN, "");
                }
                catch (Exception ex)
                {
                    _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{ex.Message}/{ex.StackTrace}");
                }
                _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Reportando Resumen ofertas");
                try
                {
                    UpdateResumenOfertas(product, "");
                }
                catch (Exception ex)
                {
                    _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{ex.Message}/{ex.StackTrace}");
                }
                _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Reportando Dimensiones");
                try
                {
                    UploadDimensiones(product.ASIN, "");
                }
                catch (Exception ex)
                {
                    _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{ex.Message}/{ex.StackTrace}");
                }
                _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Reportando Dimensiones paquete");
                try
                {
                    UploadDimensionesPaquete(product.ASIN, "");
                }
                catch (Exception ex)
                {
                    _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{ex.Message}/{ex.StackTrace}");
                }
                _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Reportando Similares");
                try
                {
                    UploadSimi(product.ASIN, "");
                }
                catch (Exception ex)
                {
                    _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{ex.Message}/{ex.StackTrace}");
                }
                _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Reportando Caracteristicas");
                try
                {
                    UploadCaracteristicas(product.ASIN, "");
                }
                catch (Exception ex)
                {
                    _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{ex.Message}/{ex.StackTrace}");
                }
                var prod = ProductosAdapter.ObtenerProducto(asin);
                prod.Netsuite = "No existe en netsuite";
                ProductosAdapter.Actualizar(prod);
                _bitacora.GuardarLinea($"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|El asin {asin} no existe en netsuite");
            }
        }

        private void UploadSimi(string asin, string internalId)
        {
            var similar = SimilaresAdapter.ObtenerSimilares(asin);
            foreach (var item in similar)
            {
                var value = ObtenerRecordId("342", "Similares", internalId, 3, "custrecord_sim_asin", item.ASINSimilar);
                var rec = new SuiteTalkWs.CustomRecord();

                var recType = new SuiteTalkWs.RecordRef
                {
                    internalId = "342",
                    name = "Similares"
                };
                rec.recType = recType;
                rec.name = "Similares";
                rec.customFieldList = CustomListSimi(internalId, item);

                if (!string.IsNullOrEmpty(value))
                {
                    rec.internalId = value;
                    var resUpdate = _client.update(rec);
                    if (resUpdate.status.isSuccess) return;
                    var res = _client.add(rec);
                    _bitacora.GuardarLinea(res.status.isSuccess
                        ? $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Correcto"
                        : $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{res.status.statusDetail.First().message}");
                    return;
                }
                var res2 = _client.add(rec);
                _bitacora.GuardarLinea(res2.status.isSuccess
                    ? $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Correcto"
                    : $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{res2.status.statusDetail.First().message}");
            }
        }

        private SuiteTalkWs.CustomFieldRef[] CustomListSimi(string internalId, Similares item)
        {
            if (!string.IsNullOrEmpty(internalId))
            {
                return new SuiteTalkWs.CustomFieldRef[]
                {
                    GenerateSelectCustomFieldRef("custrecord_sim_item", internalId, "2253"),
                    GenerarStringCustomFieldRef("custrecord_sim_asin", item.ASINSimilar),
                    GenerarStringCustomFieldRef("custrecord_sim_title", item.Title)
                };
            }
            return new SuiteTalkWs.CustomFieldRef[]
            {
                GenerarStringCustomFieldRef("custrecord_sim_asin", item.ASINSimilar),
                GenerarStringCustomFieldRef("custrecord_sim_title", item.Title)
            };
        }

        private void UploadDimensionesPaquete(string asin, string internalId)
        {
            var value = ObtenerRecordSelectId("341", "Dimensiones del paquete", internalId, 7, "custrecord_dp_item");
            var dimencion = DimensionesPaqueteAdapter.ObtenerDimensionesPaquete(asin);
            if (dimencion == null) return;
            var rec = new SuiteTalkWs.CustomRecord();

            var recType = new SuiteTalkWs.RecordRef
            {
                internalId = "341",
                name = "Dimensiones del paquete"
            };
            rec.recType = recType;
            rec.name = "Dimensiones del paquete";


            rec.customFieldList = CustomListDimensionPaquete(internalId, dimencion);

            if (!string.IsNullOrEmpty(value))
            {
                rec.internalId = value;
                var resUpdate = _client.update(rec);
                if (resUpdate.status.isSuccess) return;
                var res = _client.add(rec);
                _bitacora.GuardarLinea(res.status.isSuccess
                    ? $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Correcto"
                    : $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{res.status.statusDetail.First().message}");
                return;
            }
            var res2 = _client.add(rec);
            _bitacora.GuardarLinea(res2.status.isSuccess
                ? $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Correcto"
                : $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{res2.status.statusDetail.First().message}");
        }

        private SuiteTalkWs.CustomFieldRef[] CustomListDimensionPaquete(string internalId, DimensionesPaquete dimencion)
        {
            if (!string.IsNullOrEmpty(internalId))
            {
                return new SuiteTalkWs.CustomFieldRef[]
                {
                    GenerateSelectCustomFieldRef("custrecord_dp_item", internalId, "2246"),
                    GenerarStringCustomFieldRef("custrecord_dp_unidadmedida", dimencion.UnidadMedida),
                    GenerarStringCustomFieldRef("custrecord_dp_unidadpeso", dimencion.UnidadPeso),
                    GenerarStringCustomFieldRef("custrecord_dp_height", dimencion.Height.ToString(CultureInfo.InvariantCulture)),
                    GenerarStringCustomFieldRef("custrecord_dp_length", dimencion.Length.ToString(CultureInfo.InvariantCulture)),
                    GenerarStringCustomFieldRef("custrecord_dp_weight", dimencion.Weight.ToString(CultureInfo.InvariantCulture)),
                    GenerarStringCustomFieldRef("custrecord_dp_width", dimencion.Width.ToString(CultureInfo.InvariantCulture))
                };
            }
            return new SuiteTalkWs.CustomFieldRef[]
            {
                GenerarStringCustomFieldRef("custrecord_dp_unidadmedida", dimencion.UnidadMedida),
                GenerarStringCustomFieldRef("custrecord_dp_unidadpeso", dimencion.UnidadPeso),
                GenerarStringCustomFieldRef("custrecord_dp_height", dimencion.Height.ToString(CultureInfo.InvariantCulture)),
                GenerarStringCustomFieldRef("custrecord_dp_length", dimencion.Length.ToString(CultureInfo.InvariantCulture)),
                GenerarStringCustomFieldRef("custrecord_dp_weight", dimencion.Weight.ToString(CultureInfo.InvariantCulture)),
                GenerarStringCustomFieldRef("custrecord_dp_width", dimencion.Width.ToString(CultureInfo.InvariantCulture))
            };
        }

        private void UploadDimensiones(string asin, string internalId)
        {
            var value = ObtenerRecordSelectId("340", "Dimensiones", internalId, 7, "custrecord_dim_item");
            var dimencion = DimensionesAdapter.ObtenerDimensiones(asin);
            if (dimencion == null) return;
            var rec = new SuiteTalkWs.CustomRecord();
            var recType = new SuiteTalkWs.RecordRef
            {
                internalId = "340",
                name = "Dimensiones"
            };
            rec.recType = recType;
            rec.name = "Dimensiones";

            rec.customFieldList = CustomListDimencions(internalId, dimencion);

            if (!string.IsNullOrEmpty(value))
            {
                rec.internalId = value;
                var resUpdate = _client.update(rec);
                if (resUpdate.status.isSuccess) return;
                var res = _client.add(rec);
                _bitacora.GuardarLinea(res.status.isSuccess
                    ? $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Correcto"
                    : $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{res.status.statusDetail.First().message}");
                return;
            }
            var res2 = _client.add(rec);
            _bitacora.GuardarLinea(res2.status.isSuccess
                ? $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Correcto"
                : $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{res2.status.statusDetail.First().message}");
        }

        private SuiteTalkWs.CustomFieldRef[] CustomListDimencions(string internalId, Dimensiones dimencion)
        {
            if (!string.IsNullOrEmpty(internalId))
            {
                return new SuiteTalkWs.CustomFieldRef[]
                {
                    GenerateSelectCustomFieldRef("custrecord_dim_item", internalId, "2239"),
                    GenerarStringCustomFieldRef("custrecord_dim_unidadmedida", dimencion.UnidadMedida),
                    GenerarStringCustomFieldRef("custrecord_dim_unidadpeso", dimencion.UnidadPeso),
                    GenerarStringCustomFieldRef("custrecord_dim_height", dimencion.Height.ToString(CultureInfo.InvariantCulture)),
                    GenerarStringCustomFieldRef("custrecord_dim_length", dimencion.Length.ToString(CultureInfo.InvariantCulture)),
                    GenerarStringCustomFieldRef("custrecord_dim_weight", dimencion.Weight.ToString(CultureInfo.InvariantCulture)),
                    GenerarStringCustomFieldRef("custrecord_widht", dimencion.Width.ToString(CultureInfo.InvariantCulture))
                };
            }
            return new SuiteTalkWs.CustomFieldRef[]
            {
                GenerarStringCustomFieldRef("custrecord_dim_unidadmedida", dimencion.UnidadMedida),
                GenerarStringCustomFieldRef("custrecord_dim_unidadpeso", dimencion.UnidadPeso),
                GenerarStringCustomFieldRef("custrecord_dim_height", dimencion.Height.ToString(CultureInfo.InvariantCulture)),
                GenerarStringCustomFieldRef("custrecord_dim_length", dimencion.Length.ToString(CultureInfo.InvariantCulture)),
                GenerarStringCustomFieldRef("custrecord_dim_weight", dimencion.Weight.ToString(CultureInfo.InvariantCulture)),
                GenerarStringCustomFieldRef("custrecord_widht", dimencion.Width.ToString(CultureInfo.InvariantCulture))
            };
        }

        private void UploadCaracteristicas(string asin, string internalId)
        {
            var caract = CaracteristicasAdapter.ObtenerCaracteristicas(asin);
            foreach (var item in caract)
            {
                var value = ObtenerRecordId("345", "Buybox caracteristicas", internalId, 1, "custrecord_bc_caracteristicas", item.Caracteristica);
                var rec = new SuiteTalkWs.CustomRecord();

                var recType = new SuiteTalkWs.RecordRef
                {
                    internalId = "345",
                    name = "Buybox caracteristicas"
                };
                rec.recType = recType;
                rec.name = "Buybox caracteristicas";

                rec.customFieldList = CustomListCaract(internalId, item);

                if (!string.IsNullOrEmpty(value))
                {
                    rec.internalId = value;
                    var resUpdate = _client.update(rec);
                    if (resUpdate.status.isSuccess) continue;
                    var res = _client.add(rec);
                    _bitacora.GuardarLinea(res.status.isSuccess
                        ? $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Correcto"
                        : $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{res.status.statusDetail.First().message}");
                    continue;
                }
                var res2 = _client.add(rec);
                _bitacora.GuardarLinea(res2.status.isSuccess
                    ? $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Correcto"
                    : $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{res2.status.statusDetail.First().message}");
            }
        }

        private SuiteTalkWs.CustomFieldRef[] CustomListCaract(string internalId, Caracteristicas item)
        {
            if (!string.IsNullOrEmpty(internalId))
            {
                return new SuiteTalkWs.CustomFieldRef[]
                {
                    GenerateSelectCustomFieldRef("custrecord_bc_item", internalId, "2267"),
                    GenerarStringCustomFieldRef("custrecord_bc_caracteristicas", item.Caracteristica)
                };
            }
            return new SuiteTalkWs.CustomFieldRef[]
            {
                GenerarStringCustomFieldRef("custrecord_bc_caracteristicas", item.Caracteristica)
            };
        }

        private void UpdateResumenOfertas(Producto prod, string internalId)
        {
            var value = ObtenerRecordSelectId("344", "Resúmen Ofertas", internalId, 8, "custrecord_ro_item");
            var resumen = ResumenAdapter.ObtenerResumen(prod.ASIN);
            if (resumen == null) return;
            var rec = new SuiteTalkWs.CustomRecord();
            var recType = new SuiteTalkWs.RecordRef
            {
                internalId = "344",
                name = "Resúmen Ofertas"
            };
            rec.recType = recType;
            rec.name = "Resúmen Ofertas";

            rec.customFieldList = CustomListResumenOfertas(internalId, resumen);

            if (!string.IsNullOrEmpty(value))
            {
                rec.internalId = value;
                var resUpdate = _client.update(rec);
                if (resUpdate.status.isSuccess) return;
                var res = _client.add(rec);
                _bitacora.GuardarLinea(res.status.isSuccess
                    ? $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Correcto"
                    : $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{res.status.statusDetail.First().message}");
                return;
            }
            var res2 = _client.add(rec);
            _bitacora.GuardarLinea(res2.status.isSuccess
                ? $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Correcto"
                : $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{res2.status.statusDetail.First().message}");
        }

        private SuiteTalkWs.CustomFieldRef[] CustomListResumenOfertas(string internalId, Resumen resumen)
        {
            if (!string.IsNullOrEmpty(internalId))
            {
                return new SuiteTalkWs.CustomFieldRef[]
                {
                    GenerateSelectCustomFieldRef("custrecord_ro_item", internalId, "2259"),
                    GenerarStringCustomFieldRef("custrecord_ro_lowestprice", resumen.LowestPrice),
                    GenerarStringCustomFieldRef("custrecord_ro_currencycode", resumen.CurrencyCode),
                    GenerarStringCustomFieldRef("custrecord_ro_formattedprice", resumen.FormattedPrice),
                    GenerarStringCustomFieldRef("custrecord_ro_totalnew", resumen.TotalNew.ToString()),
                    GenerarStringCustomFieldRef("custrecord_ro_totalused", resumen.TotalUsed.ToString()),
                    GenerarStringCustomFieldRef("custrecord_ro_totalcollectible", resumen.TotalCollectible.ToString()),
                    GenerarStringCustomFieldRef("custrecord_ro_totalrefurbished", resumen.TotalRefurbished.ToString())
                };
            }
            return new SuiteTalkWs.CustomFieldRef[]
            {
                GenerarStringCustomFieldRef("custrecord_ro_lowestprice", resumen.LowestPrice),
                GenerarStringCustomFieldRef("custrecord_ro_currencycode", resumen.CurrencyCode),
                GenerarStringCustomFieldRef("custrecord_ro_formattedprice", resumen.FormattedPrice),
                GenerarStringCustomFieldRef("custrecord_ro_totalnew", resumen.TotalNew.ToString()),
                GenerarStringCustomFieldRef("custrecord_ro_totalused", resumen.TotalUsed.ToString()),
                GenerarStringCustomFieldRef("custrecord_ro_totalcollectible", resumen.TotalCollectible.ToString()),
                GenerarStringCustomFieldRef("custrecord_ro_totalrefurbished", resumen.TotalRefurbished.ToString())
            };
        }

        private void UpdateBuyboxInfo(string asin, string internalId)
        {
            var buybox = BuyBoxAdapter.ObtenerBuyBox(asin);
            foreach (var item in buybox)
            {
                var value = ObtenerRecordId("339", "BuyBox Info", internalId, 11, "custrecord_bb_item");
                var rec = new SuiteTalkWs.CustomRecord();
                var recType = new SuiteTalkWs.RecordRef
                {
                    internalId = "339",
                    name = "BuyBox Info"
                };
                rec.recType = recType;
                rec.name = "BuyBox Info";
                rec.customFieldList = CustomListBuyboxInfo(item, internalId);
                if (!string.IsNullOrEmpty(value))
                {
                    rec.internalId = value;
                    var resUpdate = _client.update(rec);
                    if (resUpdate.status.isSuccess) continue;
                    var res = _client.add(rec);
                    _bitacora.GuardarLinea(res.status.isSuccess
                        ? $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Correcto"
                        : $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{res.status.statusDetail.First().message}");
                    continue;
                }
                var res2 = _client.add(rec);
                _bitacora.GuardarLinea(res2.status.isSuccess
                    ? $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Correcto"
                    : $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{res2.status.statusDetail.First().message}");
            }
        }

        private SuiteTalkWs.CustomFieldRef[] CustomListBuyboxInfo(BuyBox buybox, string internalId)
        {
            if (!string.IsNullOrEmpty(internalId))
            {
                return new SuiteTalkWs.CustomFieldRef[]
            {
                GenerateSelectCustomFieldRef("custrecord_bb_item", internalId, "2227"),
                GenerarStringCustomFieldRef("custrecord_bb_condition", buybox.Condition),
                GenerarStringCustomFieldRef("custrecord_bb_merchant", buybox.Merchant),
                GenerarStringCustomFieldRef("custrecord_bb_amount", buybox.Amount),
                GenerarStringCustomFieldRef("custrecord_bb_currencycode", buybox.CurrencyCode),
                GenerarStringCustomFieldRef("custrecord_bb_formattedprice", buybox.FormattedPrice),
                GenerarStringCustomFieldRef("custrecord_bb_availability", buybox.Availability),
                GenerarStringCustomFieldRef("custrecord_bb_availabilitytype", buybox.AvailabilityType),
                GenerarStringCustomFieldRef("custrecord_bb_minimumhours", buybox.MinimumHours.ToString()),
                GenerarStringCustomFieldRef("custrecord_bb_maximumhours", buybox.MaximumHours.ToString()),
                GenerarStringCustomFieldRef("custrecord_bb_isiligibleforsupersaveship", buybox.IsEligibleForSuperSaveShipping.ToString()),
                GenerarStringCustomFieldRef("custrecord_bb_iseeligibleforprime", buybox.IseEligibleForPrime.ToString())
            };
            }
            return new SuiteTalkWs.CustomFieldRef[]
            {
                GenerarStringCustomFieldRef("custrecord_bb_condition", buybox.Condition),
                GenerarStringCustomFieldRef("custrecord_bb_merchant", buybox.Merchant),
                GenerarStringCustomFieldRef("custrecord_bb_amount", buybox.Amount),
                GenerarStringCustomFieldRef("custrecord_bb_currencycode", buybox.CurrencyCode),
                GenerarStringCustomFieldRef("custrecord_bb_formattedprice", buybox.FormattedPrice),
                GenerarStringCustomFieldRef("custrecord_bb_availability", buybox.Availability),
                GenerarStringCustomFieldRef("custrecord_bb_availabilitytype", buybox.AvailabilityType),
                GenerarStringCustomFieldRef("custrecord_bb_minimumhours", buybox.MinimumHours.ToString()),
                GenerarStringCustomFieldRef("custrecord_bb_maximumhours", buybox.MaximumHours.ToString()),
                GenerarStringCustomFieldRef("custrecord_bb_isiligibleforsupersaveship", buybox.IsEligibleForSuperSaveShipping.ToString()),
                GenerarStringCustomFieldRef("custrecord_bb_iseeligibleforprime", buybox.IseEligibleForPrime.ToString())
            };
        }

        private void UpdateBuybox(Producto prod, string internalId)
        {
            var value = ObtenerRecordId("338", "Buybox", prod.ASIN, 33, "custrecord_ib_asin");
            var rec = new SuiteTalkWs.CustomRecord();

            var recType = new SuiteTalkWs.RecordRef
            {
                internalId = "338",
                name = "Buybox"
            };
            rec.recType = recType;
            rec.name = "Buybox";
            rec.customFieldList = CustomListBuybox(prod, internalId);
            if (!string.IsNullOrEmpty(value))
            {
                rec.internalId = value;
                var resUpdate = _client.update(rec);
                if (resUpdate.status.isSuccess) return;
                var res = _client.add(rec);
                _bitacora.GuardarLinea(res.status.isSuccess
                    ? $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Correcto"
                    : $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{res.status.statusDetail.First().message}");
                return;
            }
            var res2 = _client.add(rec);
            _bitacora.GuardarLinea(res2.status.isSuccess
                ? $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Correcto"
                : $"{DateTime.Now:yyyy-MM-dd hh:mm:ss}|Error|{res2.status.statusDetail.First().message}");
        }

        private SuiteTalkWs.CustomFieldRef[] CustomListBuybox(Producto prod, string internalId)
        {
            if (!string.IsNullOrEmpty(internalId))
            {
                return new SuiteTalkWs.CustomFieldRef[]
            {
                GenerateSelectCustomFieldRef("custrecord_item", internalId, "2186"),
                GenerarStringCustomFieldRef("custrecord_ib_asin", prod.ASIN),
                GenerarStringCustomFieldRef("custrecord_ib_offers", prod.Offers),
                GenerarStringCustomFieldRef("custrecord_ib_salesrank", prod.SalesRank.ToString()),
                GenerarStringCustomFieldRef("custrecord_ib_smallimage", prod.SmallImage),
                GenerarStringCustomFieldRef("custrecord_ib_mediumimage", prod.MediumImage),
                GenerarStringCustomFieldRef("custrecord_ib_largeimage", prod.LargeImage),
                GenerarStringCustomFieldRef("custrecord_ib_binding", prod.Binding),
                GenerarStringCustomFieldRef("custrecord_ib_brand", prod.Brand),
                GenerarStringCustomFieldRef("custrecord_ib_clothingsize", prod.ClothingSize),
                GenerarStringCustomFieldRef("custrecord_ib_color", prod.Color),
                GenerarStringCustomFieldRef("custrecord_ib_department", prod.Department),
                GenerarStringCustomFieldRef("custrecord_ib_ean", prod.EAN),
                GenerarStringCustomFieldRef("custrecord_ib_isadultproduct", prod.isAdultProduct.ToString()),
                GenerarStringCustomFieldRef("custrecord_isautographed", prod.isAutographed.ToString()),
                GenerarStringCustomFieldRef("custrecord_ib_ismemorabilia", prod.isMemorabilia.ToString()),
                GenerarStringCustomFieldRef("custrecord_ib_label", prod.Label),
                GenerarStringCustomFieldRef("custrecord_ib_legaldisclaimer", prod.LegalDisclaimer),
                GenerarStringCustomFieldRef("custrecord_ib_manufacture", prod.Manufacture),
                GenerarStringCustomFieldRef("custrecord_ib_model", prod.Model),
                GenerarStringCustomFieldRef("custrecord_ib_mpn", prod.MPN),
                GenerarStringCustomFieldRef("custrecord_ib_numberitems", prod.NumberItems.ToString()),
                GenerarStringCustomFieldRef("custrecord_ib_packagequantity", prod.PackageQuantity.ToString()),
                GenerarStringCustomFieldRef("custrecord_ib_partnumber", prod.PartNumber),
                GenerarStringCustomFieldRef("custrecord_ib_productgroup", prod.ProductGroup),
                GenerarStringCustomFieldRef("custrecord_ib_prodcuttypename", prod.ProdcutTypeName),
                GenerarStringCustomFieldRef("custrecord_ib_publisher", prod.Publisher),
                GenerarStringCustomFieldRef("custrecord_ib_releasedate", prod.ReleaseDate),
                GenerarStringCustomFieldRef("custrecord_ib_size", prod.Size),
                GenerarStringCustomFieldRef("custrecord_ib_studio", prod.Studio),
                GenerarStringCustomFieldRef("custrecord_ib_title", prod.Title),
                GenerarStringCustomFieldRef("custrecord_ib_upc", prod.UPC),
                GenerarStringCustomFieldRef("custrecord_ib_amount", prod.Amount),
                GenerarStringCustomFieldRef("custrecord_ib_currencycode", prod.CurrencyCode),
                GenerarStringCustomFieldRef("custrecord_ib_formattedprice", prod.FormattedPrice),
                GenerarStringCustomFieldRef("custrecord_ib_actualizacion", prod.Actualizacion.ToString())
            };
            }
            return new SuiteTalkWs.CustomFieldRef[]
            {
                GenerarStringCustomFieldRef("custrecord_ib_asin", prod.ASIN),
                GenerarStringCustomFieldRef("custrecord_ib_offers", prod.Offers),
                GenerarStringCustomFieldRef("custrecord_ib_salesrank", prod.SalesRank.ToString()),
                GenerarStringCustomFieldRef("custrecord_ib_smallimage", prod.SmallImage),
                GenerarStringCustomFieldRef("custrecord_ib_mediumimage", prod.MediumImage),
                GenerarStringCustomFieldRef("custrecord_ib_largeimage", prod.LargeImage),
                GenerarStringCustomFieldRef("custrecord_ib_binding", prod.Binding),
                GenerarStringCustomFieldRef("custrecord_ib_brand", prod.Brand),
                GenerarStringCustomFieldRef("custrecord_ib_clothingsize", prod.ClothingSize),
                GenerarStringCustomFieldRef("custrecord_ib_color", prod.Color),
                GenerarStringCustomFieldRef("custrecord_ib_department", prod.Department),
                GenerarStringCustomFieldRef("custrecord_ib_ean", prod.EAN),
                GenerarStringCustomFieldRef("custrecord_ib_isadultproduct", prod.isAdultProduct.ToString()),
                GenerarStringCustomFieldRef("custrecord_isautographed", prod.isAutographed.ToString()),
                GenerarStringCustomFieldRef("custrecord_ib_ismemorabilia", prod.isMemorabilia.ToString()),
                GenerarStringCustomFieldRef("custrecord_ib_label", prod.Label),
                GenerarStringCustomFieldRef("custrecord_ib_legaldisclaimer", prod.LegalDisclaimer),
                GenerarStringCustomFieldRef("custrecord_ib_manufacture", prod.Manufacture),
                GenerarStringCustomFieldRef("custrecord_ib_model", prod.Model),
                GenerarStringCustomFieldRef("custrecord_ib_mpn", prod.MPN),
                GenerarStringCustomFieldRef("custrecord_ib_numberitems", prod.NumberItems.ToString()),
                GenerarStringCustomFieldRef("custrecord_ib_packagequantity", prod.PackageQuantity.ToString()),
                GenerarStringCustomFieldRef("custrecord_ib_partnumber", prod.PartNumber),
                GenerarStringCustomFieldRef("custrecord_ib_productgroup", prod.ProductGroup),
                GenerarStringCustomFieldRef("custrecord_ib_prodcuttypename", prod.ProdcutTypeName),
                GenerarStringCustomFieldRef("custrecord_ib_publisher", prod.Publisher),
                GenerarStringCustomFieldRef("custrecord_ib_releasedate", prod.ReleaseDate),
                GenerarStringCustomFieldRef("custrecord_ib_size", prod.Size),
                GenerarStringCustomFieldRef("custrecord_ib_studio", prod.Studio),
                GenerarStringCustomFieldRef("custrecord_ib_title", prod.Title),
                GenerarStringCustomFieldRef("custrecord_ib_upc", prod.UPC),
                GenerarStringCustomFieldRef("custrecord_ib_amount", prod.Amount),
                GenerarStringCustomFieldRef("custrecord_ib_currencycode", prod.CurrencyCode),
                GenerarStringCustomFieldRef("custrecord_ib_formattedprice", prod.FormattedPrice),
                GenerarStringCustomFieldRef("custrecord_ib_actualizacion", prod.Actualizacion.ToString())
            };
        }

        public void ActualizarCampos(Dictionary<string, string> campos, string asin)
        {
            if (campos.Count > 0)
            {
                var custFieldList = new SuiteTalkWs.CustomFieldRef[campos.Count];
                var cont = 0;
                foreach (var campo in campos)
                {
                    if (campo.Key == Box)
                    {
                        var fieldRef = GenerarBooleanCustomFieldRef(campo.Key, campo.Value);
                        custFieldList[cont] = fieldRef;
                    }
                    else
                    {
                        var fieldRef = GenerarStringCustomFieldRef(campo.Key, campo.Value);
                        custFieldList[cont] = fieldRef;
                    }
                    cont++;
                }

                var item = new SuiteTalkWs.InventoryItem();

                var internalId = ObtenerId(asin);

                if (!internalId.Equals("-1"))
                {
                    item.internalId = internalId;
                    item.customFieldList = custFieldList;

                    var response = _client.update(item);

                    if (!response.status.isSuccess)
                    {
                        LstErrores.Add(ReportarErrores(response.status));
                        StatusNotificacion = false;
                    }
                    else
                    {
                        StatusNotificacion = true;
                    }
                }

            }
        }

        private string ObtenerRecordSelectId(string internalId, string name, string search, int totalColums, string columName, string search2 = "")
        {
            var customRecordSearch = new SuiteTalkWs.CustomRecordSearch();
            var customRecordSearchBasic = new SuiteTalkWs.CustomRecordSearchBasic();
            var recType = new SuiteTalkWs.RecordRef
            {
                internalId = internalId,
                name = name
            };
            customRecordSearchBasic.recType = recType;
            var customRecordName = new SuiteTalkWs.SearchStringField
            {
                @operator = SuiteTalkWs.SearchStringFieldOperator.contains,
                operatorSpecified = true,
                searchValue = search
            };
            customRecordSearchBasic.name = customRecordName;

            customRecordSearch.basic = customRecordSearchBasic;
            var response = _client.search(customRecordSearch);

            // Process response
            if (response.status.isSuccess)
            {
                foreach (var item in response.recordList)
                {
                    var custom = (SuiteTalkWs.CustomRecord)item;
                    //if (custom.customFieldList.Length == totalColums)
                    //{
                        foreach (var item2 in custom.customFieldList.Where(x => x.GetType() == typeof(SuiteTalkWs.SelectCustomFieldRef)))
                        {
                            try
                            {
                                var value = (SuiteTalkWs.SelectCustomFieldRef)item2;
                                if (search2 == "")
                                {
                                    if (value.scriptId == columName && value.value.internalId == search)
                                        return custom.internalId;
                                }
                                else
                                {
                                    if (value.scriptId == columName && value.value.internalId == search)
                                        return custom.internalId;
                                }
                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                        }
                    //}
                }
            }
            else
            {
                Console.WriteLine(response.status);
            }

            return "";
        }

        private string ObtenerRecordId(string internalId, string name, string search, int totalColums, string columName, string search2 = "")
        {
            var customRecordSearch = new SuiteTalkWs.CustomRecordSearch();
            var customRecordSearchBasic = new SuiteTalkWs.CustomRecordSearchBasic();
            var recType = new SuiteTalkWs.RecordRef
            {
                internalId = internalId,
                name = name
            };
            customRecordSearchBasic.recType = recType;
            var customRecordName = new SuiteTalkWs.SearchStringField
            {
                @operator = SuiteTalkWs.SearchStringFieldOperator.contains,
                operatorSpecified = true,
                searchValue = search
            };
            customRecordSearchBasic.name = customRecordName;

            customRecordSearch.basic = customRecordSearchBasic;
            var response = _client.search(customRecordSearch);

            // Process response
            if (response.status.isSuccess)
            {
                foreach (var item in response.recordList)
                {
                    var custom = (SuiteTalkWs.CustomRecord)item;
                    //if (custom.customFieldList.Length == totalColums)
                    //{
                        foreach (var item2 in custom.customFieldList.Where(x => x.GetType() == typeof(SuiteTalkWs.StringCustomFieldRef) && x.scriptId == columName))
                        {
                            try
                            {
                                var value = (SuiteTalkWs.StringCustomFieldRef)item2;
                                if (search2 == "")
                                {
                                    if (value.value == search)
                                        return custom.internalId;
                                }
                                else
                                {
                                    if (value.value == search2)
                                        return custom.internalId;
                                }
                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                        }
                    //}
                }
            }
            else
            {
                Console.WriteLine(response.status);
            }

            return "";
        }

        private string ObtenerId(string asin)
        {
            var itemSearch = new SuiteTalkWs.ItemSearch();

            var customFieldList = new List<SuiteTalkWs.SearchCustomField>
            {
                new SuiteTalkWs.SearchStringCustomField
                {
                    scriptId = Asin,
                    searchValue =asin,
                    @operator = SuiteTalkWs.SearchStringFieldOperator.@is,
                    operatorSpecified = true
                }}.ToArray();

            var itemBasic = new SuiteTalkWs.ItemSearchBasic { customFieldList = customFieldList };
            itemSearch.basic = itemBasic;

            var res = _client.search(itemSearch);
            var type = res.recordList.FirstOrDefault()?.GetType();
            if (type == typeof(SuiteTalkWs.InventoryItem))
            {
                var item = (SuiteTalkWs.InventoryItem)res.recordList.FirstOrDefault();
                return item != null ? item.internalId : "-1";
            }
            if (type == typeof(SuiteTalkWs.NonInventorySaleItem))
            {
                var item = (SuiteTalkWs.NonInventorySaleItem)res.recordList.FirstOrDefault();
                return item != null ? item.internalId : "-1";
            }

            if (type == typeof(SuiteTalkWs.KitItem))
            {
                var item = (SuiteTalkWs.KitItem)res.recordList.FirstOrDefault();
                return item != null ? item.internalId : "-1";
            }

            return null;
        }

        private bool Autenticar()
        {
            var passport = new SuiteTalkWs.Passport();
            var role = new SuiteTalkWs.RecordRef();
            passport.account = ConfigurationManager.AppSettings["SuiteAccount"];
            passport.email = ConfigurationManager.AppSettings["SuiteEmail"];
            role.internalId = ConfigurationManager.AppSettings["SuiteInternalId"];

            passport.role = role;
            passport.password = ConfigurationManager.AppSettings["SuitePassword"];

            _client.applicationInfo =
                new SuiteTalkWs.ApplicationInfo
                {
                    applicationId = ConfigurationManager.AppSettings["SuiteApplicationId"]
                };

            var response = _client.login(passport);
            if (response.status.isSuccess)
                return true;
            LstErrores.Add(ReportarErrores(response.status));
            return false;
        }

        private SuiteTalkWs.BooleanCustomFieldRef GenerarBooleanCustomFieldRef(string id, string valor)
        {
            return new SuiteTalkWs.BooleanCustomFieldRef
            {
                value = Convert.ToBoolean(valor),
                scriptId = id
            };
        }

        private SuiteTalkWs.StringCustomFieldRef GenerarStringCustomFieldRef(string id, string valor)
        {
            return new SuiteTalkWs.StringCustomFieldRef
            {
                value = valor ?? "",
                scriptId = id
            };
        }

        private SuiteTalkWs.SelectCustomFieldRef GenerateSelectCustomFieldRef(string id, string value, string internalId)
        {
            return new SuiteTalkWs.SelectCustomFieldRef
            {
                internalId = internalId,
                scriptId = id,
                value = new SuiteTalkWs.ListOrRecordRef
                {
                    internalId = value,
                    name = "Artículo"
                }
            };
        }

        private string ReportarErrores(SuiteTalkWs.Status status)
        {
            var sb = new StringBuilder();

            foreach (var detail in status.statusDetail)
            {
                sb.Append("[Code=" + detail.code + "] " + detail.message + "\n");
            }

            return sb.ToString();
        }

        public void CerrarSesion()
        {
            _client.logout();
        }
    }
}