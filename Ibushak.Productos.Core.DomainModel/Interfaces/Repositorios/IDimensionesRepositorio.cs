using Ibushak.Productos.Core.DomainModel.Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ibushak.Productos.Core.DomainModel.Interfaces.Repositorios
{
    public interface IDimensionesRepositorio : IRepositorio<Dimensiones>
    {
        IEnumerable<Dimensiones> obtenerDimensionesActualizados(IEnumerable<string> asin);
    }
}
