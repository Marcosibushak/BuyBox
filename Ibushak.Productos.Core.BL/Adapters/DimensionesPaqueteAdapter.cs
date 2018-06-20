using Ibushak.Productos.Core.DAL;
using Ibushak.Productos.Core.DomainModel.Datos;
using System.Collections.Generic;
using System.Linq;

namespace Ibushak.Productos.Core.BL.Adapters
{
    public static class DimensionesPaqueteAdapter
    {
        public static DimensionesPaquete ObtenerDimensionesPaquete(string asin)
        {
            DimensionesPaquete oDimensionesPaquete;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
                oDimensionesPaquete = unidadDeTrabajo.DimensionesPaquete.Obtener(asin);
            return oDimensionesPaquete;
        }

        public static IEnumerable<DimensionesPaquete> ObtenerCaracteristicasActualizados(IEnumerable<string> asin)
        {
            IEnumerable<DimensionesPaquete> dimensionesPaquete;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
                dimensionesPaquete = unidadDeTrabajo.DimensionesPaquete.obtenerDimensionesPaqueteActualizados(asin).ToList();
            return dimensionesPaquete;
        }

        public static void Actualizar(DimensionesPaquete dimensionesPaquete)
        {
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
            {
                unidadDeTrabajo.DimensionesPaquete.actualizar(dimensionesPaquete);
                unidadDeTrabajo.guardarCambios();
            }
        }
    }
}