using Ibushak.Productos.Core.DAL;
using Ibushak.Productos.Core.DomainModel.Ofertas;
using System.Collections.Generic;
using System.Linq;

namespace Ibushak.Productos.Core.BL.Adapters
{
    public static class BuyBoxAdapter
    {
        public static BuyBox ObtenerBuyBox(string asin, string condicion)
        {
            BuyBox oBuyBox;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
                oBuyBox = unidadDeTrabajo.BuyBox.Obtener(asin, condicion);
            return oBuyBox;
        }

        public static List<BuyBox> ObtenerBuyBox(string asin)
        {
            List<BuyBox> oBuyBox;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
                oBuyBox = unidadDeTrabajo.BuyBox.ObtenerList(asin);
            return oBuyBox;
        }

        public static IEnumerable<BuyBox> ObtenerCaracteristicasActualizados(IEnumerable<string> asin)
        {
            IEnumerable<BuyBox> buyBox;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
                buyBox = unidadDeTrabajo.BuyBox.ObtenerBuyBoxActualizados(asin).ToList();
            return buyBox;
        }

        public static void ActualizarBuyBox(List<BuyBox> lstBuyBox)
        {
            var asin = lstBuyBox.First().ASIN;
            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
            {
                unidadDeTrabajo.BuyBox.BorrarTodo(asin);
                unidadDeTrabajo.guardarCambios();

                lstBuyBox.ForEach(b => unidadDeTrabajo.BuyBox.agregar(b));

                unidadDeTrabajo.guardarCambios();
            }
        }
    }
}
