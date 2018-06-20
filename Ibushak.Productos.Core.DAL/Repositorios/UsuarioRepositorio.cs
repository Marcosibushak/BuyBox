using Ibushak.Productos.Core.DomainModel.Interfaces.Repositorios;
using Ibushak.Productos.Core.DomainModel.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ibushak.Productos.Core.DAL.Repositorios
{
    public class UsuarioRepositorio : Repositorio<Usuario>, IUsuarioRepositorio
    {

        public UsuarioRepositorio(IbushakProductosContext context) : base(context)
        {

        }

        public IbushakProductosContext IbushakProductosContext
        {
            get { return context as IbushakProductosContext; }
        }
    }
}
