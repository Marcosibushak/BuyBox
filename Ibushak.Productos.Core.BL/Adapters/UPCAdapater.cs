using Ibushak.Productos.Core.DAL;
using Ibushak.Productos.Core.DomainModel.Catologos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ibushak.Productos.Core.BL.Adapters
{
    public static class UPCAdapater
    {
        public static bool Existe(string upc)
        {
            bool existe;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
            {
                var cont = unidadDeTrabajo.UPC.buscar(u => u.Id == upc).AsQueryable().Count();

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
                    unidadDeTrabajo.UPC.borrarTodo();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void agregarUPCs(List<UPC> lstUPC)
        {
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
            {
                lstUPC.ForEach(u =>
                {
                    unidadDeTrabajo.UPC.agregar(u);
                });

                unidadDeTrabajo.guardarCambios();
            }
        }
    }
}
