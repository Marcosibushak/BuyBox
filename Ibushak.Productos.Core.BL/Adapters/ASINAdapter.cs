using System;
using Ibushak.Productos.Core.DAL;
using Ibushak.Productos.Core.DomainModel.Catologos;
using System.Collections.Generic;
using System.Linq;

namespace Ibushak.Productos.Core.BL.Adapters
{
    public static class ASINAdapter
    {
        public static IEnumerable<ASIN> GetAllAsins()
        {
            IEnumerable<ASIN> productos;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
                productos = unidadDeTrabajo.ASIN.obtenerTodos().ToList();
            return productos;
        }

        public static bool Existe(string asin)
        {
            bool existe;

            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
            {
                var cont = unidadDeTrabajo.ASIN.buscar(a => a.Id == asin).AsQueryable().Count();

                existe = cont > 0;
            }
            return existe;
        }

        public static void DeleteTable()
        {
            try
            {
                using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
                {
                    unidadDeTrabajo.ASIN.borrarTodo();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void AgregarAsiNs(List<ASIN> lstAsin)
        {
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
            {
                lstAsin.ForEach(a =>
                {
                    unidadDeTrabajo.ASIN.agregar(a);
                });
                unidadDeTrabajo.guardarCambios();
            }
        }
    }
}