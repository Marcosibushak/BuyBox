using Ibushak.Productos.Core.DAL;
using Ibushak.Productos.Core.DomainModel.Datos;
using System.Collections.Generic;
using System.Linq;

namespace Ibushak.Productos.Core.BL.Adapters
{
    public static class DimensionesAdapter
    {
        public static Dimensiones ObtenerDimensiones(string asin)
        {
            Dimensiones oDimensiones;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
                oDimensiones = unidadDeTrabajo.Dimensiones.Obtener(asin);
            return oDimensiones;
        }

        public static IEnumerable<Dimensiones> ObtenerCaracteristicasActualizados(IEnumerable<string> asin)
        {
            IEnumerable<Dimensiones> dimensiones;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
                dimensiones = unidadDeTrabajo.Dimensiones.obtenerDimensionesActualizados(asin).ToList();
            return dimensiones;
        }

        public static void Actualizar(Dimensiones dimensiones)
        {
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
            {
                unidadDeTrabajo.Dimensiones.actualizar(dimensiones);
                unidadDeTrabajo.guardarCambios();
            }
        }
    }
}