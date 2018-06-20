using Ibushak.Productos.Core.DomainModel.Ofertas;
using System.Collections.Generic;

namespace Ibushak.Productos.Core.DomainModel.Interfaces.Repositorios
{
    public interface IBuyBoxRepositorio : IRepositorio<BuyBox>
    {
        BuyBox Obtener(string id, string condicion);
        void BorrarTodo(string id);
        List<BuyBox> ObtenerList(string asin);
        IEnumerable<BuyBox> ObtenerBuyBoxActualizados(IEnumerable<string> asin);
    }
}
