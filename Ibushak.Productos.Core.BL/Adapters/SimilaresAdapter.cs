using Ibushak.Productos.Core.DAL;
using Ibushak.Productos.Core.DomainModel.Datos;
using System.Collections.Generic;
using System.Linq;

namespace Ibushak.Productos.Core.BL.Adapters
{
    public static class SimilaresAdapter
    {
        public static List<Similares> ObtenerSimilares(string asin)
        {
            List<Similares> lstSimilares;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
                lstSimilares = unidadDeTrabajo.Similares.buscar(s => s.ASIN == asin).ToList();
            return lstSimilares;
        }

        public static IEnumerable<Similares> ObtenerCaracteristicasActualizados(IEnumerable<string> asin)
        {
            IEnumerable<Similares> similares;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
                similares = unidadDeTrabajo.Similares.obtenerSimilaresActualizados(asin).ToList();
            return similares;
        }

        public static void Actualizar(List<Similares> lstSimilares)
        {
            var asin = lstSimilares.First().ASIN;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
            {
                unidadDeTrabajo.Similares.borrarTodo(asin);
                unidadDeTrabajo.guardarCambios();
                lstSimilares.ForEach(s => unidadDeTrabajo.Similares.agregar(s));
                unidadDeTrabajo.guardarCambios();
            }
        }
    }
}