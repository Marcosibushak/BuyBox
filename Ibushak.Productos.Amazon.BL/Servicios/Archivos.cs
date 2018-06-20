using Ibushak.Productos.Core.BL.Adapters;
using Ibushak.Productos.Core.DomainModel.Catologos;
using Ibushak.Productos.Amazon.BL.Extensiones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using OfficeOpenXml;
using System.Drawing;
using System.IO;
using Ibushak.Productos.Core.BL.Envios;

namespace Ibushak.Productos.Amazon.BL.Servicios
{
    public class Archivos
    {
        private const string Productos = "Productos";
        private const string Buybox = "BuyBox";
        private const string Caracteristicas = "Caracteristicas";
        private const string Dimensiones = "Dimensiones";
        private const string Dimensionespaquete = "DimensionesPaquete";
        private const string Resumen = "ResumenOfertas";
        private const string Similares = "Similares";
        private const string Upcs = "UPCs";

        public void GenerarProductosActualizados()
        {
            IEnumerable<Producto> lstProductos = ProductosAdapter.ObtenerProductosTodos().ToList();
            IEnumerable<string> lstAsin = lstProductos.Select(producto => producto.ASIN).ToList();

            if (!lstAsin.Any()) return;
            var lstDataTable = new List<DataTable>();
            var oCorreo = new Correo();

            var dtasins = ASINAdapter.GetAllAsins().ConvertirADataTable("asin");
            var dtProductos = lstProductos.ConvertirADataTable(Productos);
            var dtBuyBox = BuyBoxAdapter.ObtenerCaracteristicasActualizados(lstAsin).ConvertirADataTable(Buybox);
            var dtCaracteristicas = CaracteristicasAdapter.ObtenerCaracteristicasActualizados(lstAsin).ConvertirADataTable(Caracteristicas);
            var dtDimensiones = DimensionesAdapter.ObtenerCaracteristicasActualizados(lstAsin).ConvertirADataTable(Dimensiones);
            var dtDimensionesPaquete = DimensionesPaqueteAdapter.ObtenerCaracteristicasActualizados(lstAsin).ConvertirADataTable(Dimensionespaquete);
            var dtResumen = ResumenAdapter.ObtenerCaracteristicasActualizados(lstAsin).ConvertirADataTable(Resumen);
            var dtSimilares = SimilaresAdapter.ObtenerCaracteristicasActualizados(lstAsin).ConvertirADataTable(Similares);
            var dtUpCs = UPCsAdapter.ObtenerCaracteristicasActualizados(lstAsin).ConvertirADataTable(Upcs);

            lstDataTable.Add(dtasins);
            lstDataTable.Add(dtProductos);
            lstDataTable.Add(dtBuyBox);
            lstDataTable.Add(dtResumen);
            lstDataTable.Add(dtCaracteristicas);
            lstDataTable.Add(dtDimensiones);
            lstDataTable.Add(dtDimensionesPaquete);
            lstDataTable.Add(dtSimilares);
            lstDataTable.Add(dtUpCs);

            var bytes = GenerarExcel(lstDataTable);

            var nombreArchivo = $"{DateTime.Now:yyyy-MM-dd HHmm} Productos Amazon.xlsx";
            var existoso = oCorreo.EnviarMensaje(bytes, nombreArchivo);

            if (!existoso) return;
            foreach (var asin in lstAsin)
                ProductosAdapter.ActualizarBandera(asin, false);
        }

        private byte[] GenerarExcel(List<DataTable> lstDt)
        {
            byte[] bytes;
            using (var pack = new ExcelPackage())
            {
                lstDt.ForEach(dt =>
                {
                    var ws = pack.Workbook.Worksheets.Add(dt.TableName);
                    ws.Cells["A1"].LoadFromDataTable(dt, true);
                    //Format the header for column
                    using (var rng = ws.Cells[1, 1, 1, dt.Columns.Count])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;  //Set Pattern for the background to Solid
                        rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                        rng.Style.Font.Color.SetColor(Color.White);
                    }

                    if(dt.TableName != Productos)
                    {
                        var totalColumnas = dt.Columns.Count;
                        var totalRenglones = dt.Rows.Count;
                        var final = ExcelCellBase.GetAddress(totalRenglones + 1, totalColumnas);
                        var rangoTotal = "A1:" + final;
                        ws.Cells[rangoTotal].AutoFitColumns();
                    }
                });
                var ms = new MemoryStream();
                pack.SaveAs(ms);
                bytes = ms.ToArray();
                ms.Close();
            }
            return bytes;
        }
    }
}