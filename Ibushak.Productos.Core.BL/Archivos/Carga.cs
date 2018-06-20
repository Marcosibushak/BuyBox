using Ibushak.Productos.Core.BL.Adapters;
using Ibushak.Productos.Core.DomainModel.Catologos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.OleDb;

namespace Ibushak.Productos.Core.BL.Archivos
{
    public class Carga
    {
        public string ErrorMensaje { get; set; }
        public string MessageCount { get; set; }

        public bool CargarAsinUpcs(string path)
        {
            try
            {
                ASINAdapter.DeleteTable();
                UPCAdapater.DeleteTable();
                ProductosAdapter.DeleteTable();
                var lstUpc = GenerarListaAsinUpcs(path);

                var lstAsin = new List<string>();
                var lstUpcs = new List<string>();
                var countAsin = 0;
                var countUpc = 0;
                foreach (var upcs in lstUpc.Distinct())
                {
                    var asin = upcs[0];
                    var upc = upcs[1];
                    if(asin.ToLower().Trim() == "asin" || asin.ToLower().Trim() == "upc") continue;

                    if (!asin.Equals(""))
                    {
                        countAsin++;
                        if (!ASINAdapter.Existe(asin))
                            lstAsin.Add(asin.Trim());
                    }

                    if (upc.Equals("")) continue;
                    countUpc++;
                    if (UPCAdapater.Existe(upc)) continue;
                    lstUpcs.Add(upc.Trim());
                }
                
                if (lstAsin.Any())
                    ASINAdapter.AgregarAsiNs(lstAsin.Distinct(StringComparer.CurrentCultureIgnoreCase).Select(x => new ASIN {Id = x}).ToList());
                if (lstUpcs.Any())
                    UPCAdapater.agregarUPCs(lstUpcs.Distinct(StringComparer.CurrentCultureIgnoreCase).Select(x => new UPC{ Id = x}).ToList());

                File.Delete(path);
                MessageCount =
                    $"Total ASIN: {countAsin}\nIngresados en base:{lstAsin.Distinct(StringComparer.CurrentCultureIgnoreCase).Count()}\n" +
                    $"Total UPC: {countUpc}\nIngresados en base: {lstUpcs.Distinct(StringComparer.CurrentCultureIgnoreCase).Count()}";
                return true;
            }
            catch (Exception ex)
            {
                ErrorMensaje = ex.Message;
                return false;
            }
        }

        private List<string[]> GenerarListaAsinUpcs(string path)
        {
            string connectionString;
            var archivo = path.Split('.');
            var lst = new List<string[]>();
            var extension = archivo[archivo.Length - 1];
            if (extension == "xls")
                connectionString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source ={path};Extended Properties='Excel 8.0;HDR=YES;'";
            else
                connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={path};Extended Properties='Excel 12.0 Xml;HDR=NO;'";
            using (var connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                var tableInfo = connection.GetSchema("Tables");
                var hoja = tableInfo.Rows[0]["TABLE_NAME"].ToString();
                var command = new OleDbCommand($"select * from [{hoja}]", connection);
                using (var dr = command.ExecuteReader())
                {
                    while (dr != null && dr.Read())
                    {
                        string[] ids = { dr[0].ToString(), dr[1].ToString() };
                        lst.Add(ids);
                    }
                }
            }
            return lst;
        }
    }
}