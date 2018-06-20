using Ibushak.Productos.Core.DAL;
using Ibushak.Productos.Core.DomainModel.Ofertas;
using System.Collections.Generic;
using System.Linq;

namespace Ibushak.Productos.Core.BL.Adapters
{
    public static class ResumenAdapter
    {
        public static Resumen ObtenerResumen(string asin)
        {
            Resumen oResumen;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
                oResumen = unidadDeTrabajo.Resumen.Obtener(asin);
            return oResumen;
        }

        public static IEnumerable<Resumen> ObtenerCaracteristicasActualizados(IEnumerable<string> asin)
        {
            IEnumerable<Resumen> resumen;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
                resumen = unidadDeTrabajo.Resumen.obtenerResumenActualizados(asin).ToList();
            return resumen;
        }

        public static void Actualizar(Resumen resumen)
        {
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
            {
                unidadDeTrabajo.Resumen.actualizar(resumen);
                unidadDeTrabajo.guardarCambios();
            }
        }
    }
}