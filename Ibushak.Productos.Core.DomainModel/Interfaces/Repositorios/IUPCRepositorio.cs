using Ibushak.Productos.Core.DomainModel.Catologos;

namespace Ibushak.Productos.Core.DomainModel.Interfaces.Repositorios
{
    public interface IUPCRepositorio : IRepositorio<UPC>
    {
        void borrarTodo();
    }
}