using Ibushak.Productos.Core.DomainModel.Catologos;
using System.Collections.Generic;

namespace Ibushak.Productos.Core.DomainModel.Interfaces.Repositorios
{
    public interface IProductoRepositorio : IRepositorio<Producto>
    {
        IEnumerable<Producto> obtenerProductosActualizados();
        void DeleteAll();
    }
}