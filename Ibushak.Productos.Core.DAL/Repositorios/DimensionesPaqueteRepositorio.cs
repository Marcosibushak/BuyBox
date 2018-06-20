using Ibushak.Productos.Core.DomainModel.Datos;
using Ibushak.Productos.Core.DomainModel.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ibushak.Productos.Core.DAL.Repositorios
{
    public class DimensionesPaqueteRepositorio : Repositorio<DimensionesPaquete>, IDimensionesPaqueteRepositorio
    {
        public DimensionesPaqueteRepositorio(IbushakProductosContext context) : base(context)
        {
        }

        public IbushakProductosContext IbushakProductosContext
        {
            get { return context as IbushakProductosContext; }
        }

        public IEnumerable<DimensionesPaquete> obtenerDimensionesPaqueteActualizados(IEnumerable<string> asin)
        {
            var resultado = (from dimensionesPaquete in IbushakProductosContext.DimensionesPaquete
                             where asin.Contains(dimensionesPaquete.ASIN)
                             select dimensionesPaquete).AsEnumerable();

            return resultado;
        }
    }
}
