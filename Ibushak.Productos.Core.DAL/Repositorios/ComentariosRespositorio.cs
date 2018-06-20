using Ibushak.Productos.Core.DomainModel.Datos;
using Ibushak.Productos.Core.DomainModel.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ibushak.Productos.Core.DAL.Repositorios
{
    public class ComentariosRespositorio : Repositorio<Comentarios>, IComentariosRepositorio
    {
        public ComentariosRespositorio(IbushakProductosContext context) : base(context)
        {
        }

        public IbushakProductosContext IbushakProductosContext
        {
            get { return context as IbushakProductosContext; }
        }
    }
}
