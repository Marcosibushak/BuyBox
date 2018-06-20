using System;
using System.Configuration;
using System.IO;

namespace Ibushak.Productos.Core.BL.Archivos
{
    public class Bitacora
    {
        private readonly string _archivo = ConfigurationManager.AppSettings["bitacora"];

        public Bitacora()
        {
            _archivo = Path.Combine(_archivo, $"{ DateTime.Now :yyyyMMddhh} Bitacora.txt" );
            if (!File.Exists(_archivo))
                File.CreateText(_archivo).Close();
        }

        public void GuardarLinea(string mensaje)
        {
            using (var outfile = new StreamWriter(_archivo, true))
            {
                outfile.WriteLine(mensaje);
                outfile.Close();
            }
        }
    }
}