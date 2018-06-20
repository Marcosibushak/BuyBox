using Ibushak.Productos.Core.DomainModel.Datos;
using System.Collections.Generic;

namespace Ibushak.Productos.Core.DomainModel.Interfaces.Repositorios
{
    public interface ICaracteristicasRepositorio : IRepositorio<Caracteristicas>
    {
        void borrarTodo(string id);

        IEnumerable<Caracteristicas> obtenerCaracteristicasActualizados(IEnumerable<string> asin);
    }
}
