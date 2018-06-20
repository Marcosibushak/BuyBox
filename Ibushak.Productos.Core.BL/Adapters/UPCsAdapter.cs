using Ibushak.Productos.Core.DAL;
using Ibushak.Productos.Core.DomainModel.Datos;
using System.Collections.Generic;
using System.Linq;

namespace Ibushak.Productos.Core.BL.Adapters
{
    public static class UPCsAdapter
    {
        public static List<UPCs> ObtenerUpcs(string asin)
        {
            List<UPCs> lstUpcs;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
                lstUpcs = unidadDeTrabajo.UPCs.buscar(u => u.ASIN == asin).ToList();
            return lstUpcs;
        }

        public static IEnumerable<UPCs> ObtenerCaracteristicasActualizados(IEnumerable<string> asin)
        {
            IEnumerable<UPCs> upcs;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
                upcs = unidadDeTrabajo.UPCs.obtenerUPCsActualizados(asin).ToList();
            return upcs;
        }

        public static void Actualizar(List<UPCs> lstUpcs)
        {
            var asin = lstUpcs.First().ASIN;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
            {
                unidadDeTrabajo.UPCs.borrarTodo(asin);
                unidadDeTrabajo.guardarCambios();
                lstUpcs.ForEach(u => unidadDeTrabajo.UPCs.agregar(u));
                unidadDeTrabajo.guardarCambios();
            }
        }
    }
}