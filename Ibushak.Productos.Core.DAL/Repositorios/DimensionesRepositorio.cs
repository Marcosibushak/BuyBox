using Ibushak.Productos.Core.DomainModel.Datos;
using Ibushak.Productos.Core.DomainModel.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ibushak.Productos.Core.DAL.Repositorios
{
    public class DimensionesRepositorio : Repositorio<Dimensiones>, IDimensionesRepositorio
    {
        public DimensionesRepositorio(IbushakProductosContext context) : base(context)
        {
        }

        public IbushakProductosContext IbushakProductosContext
        {
            get { return context as IbushakProductosContext; }
        }

        public IEnumerable<Dimensiones> obtenerDimensionesActualizados(IEnumerable<string> asin)
        {
            var resultado = (from dimension in IbushakProductosContext.Dimensiones
                             where asin.Contains(dimension.ASIN)
                             select dimension).AsEnumerable();

            return resultado;
        }
    }
}
