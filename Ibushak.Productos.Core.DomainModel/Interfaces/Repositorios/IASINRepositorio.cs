using Ibushak.Productos.Core.DomainModel.Catologos;

namespace Ibushak.Productos.Core.DomainModel.Interfaces.Repositorios
{
    public interface IASINRepositorio : IRepositorio<ASIN>
    {
        void borrarTodo();
    }
}