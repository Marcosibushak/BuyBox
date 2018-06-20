using Ibushak.Productos.Core.DAL;
using Ibushak.Productos.Core.DomainModel.Datos;
using System.Collections.Generic;
using System.Linq;

namespace Ibushak.Productos.Core.BL.Adapters
{
    public static class CaracteristicasAdapter
    {
        public static List<Caracteristicas> ObtenerCaracteristicas(string asin)
        {
            List<Caracteristicas> lstCaracteristicas;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
                lstCaracteristicas = unidadDeTrabajo.Caracteristicas.buscar(c => c.ASIN == asin).ToList();
            return lstCaracteristicas;
        }

        public static IEnumerable<Caracteristicas> ObtenerCaracteristicasActualizados(IEnumerable<string> asin)
        {
            IEnumerable<Caracteristicas> caracteristicas;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
                caracteristicas = unidadDeTrabajo.Caracteristicas.obtenerCaracteristicasActualizados(asin).ToList();
            return caracteristicas;
        }

        public static void Actualizar(List<Caracteristicas> lstCaracteristicas)
        {
            var asin = lstCaracteristicas.First().ASIN;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
            {
                unidadDeTrabajo.Caracteristicas.borrarTodo(asin);
                unidadDeTrabajo.guardarCambios();
                lstCaracteristicas.ForEach(c => unidadDeTrabajo.Caracteristicas.agregar(c));
                unidadDeTrabajo.guardarCambios();
            }
        }
    }
}