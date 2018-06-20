using Ibushak.Productos.Core.DomainModel.Ofertas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ibushak.Productos.Core.DomainModel.Interfaces.Repositorios
{
    public interface IResumenRepositorio : IRepositorio<Resumen>
    {
        IEnumerable<Resumen> obtenerResumenActualizados(IEnumerable<string> asin);
    }
}
