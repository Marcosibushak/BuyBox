using Ibushak.Productos.Core.DomainModel.Interfaces.Repositorios;
using Ibushak.Productos.Core.DomainModel.Ofertas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ibushak.Productos.Core.DAL.Repositorios
{
    public class ResumenRepositorio : Repositorio<Resumen>, IResumenRepositorio
    {
        public ResumenRepositorio(IbushakProductosContext context) : base(context)
        {
        }

        public IbushakProductosContext IbushakProductosContext
        {
            get { return context as IbushakProductosContext; }
        }

        public IEnumerable<Resumen> obtenerResumenActualizados(IEnumerable<string> asin)
        {
            var resultado = (from resumen in IbushakProductosContext.Resumen
                             where asin.Contains(resumen.ASIN)
                             select resumen).AsEnumerable();

            return resultado;
        }
    }
}
