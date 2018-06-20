using Ibushak.Productos.Core.DomainModel.Catologos;
using Ibushak.Productos.Core.DomainModel.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Ibushak.Productos.Core.DAL.Repositorios
{
    public class ASINRepositorio : Repositorio<ASIN>, IASINRepositorio
    {
        public ASINRepositorio(IbushakProductosContext context) : base(context)
        {
        }

        public IbushakProductosContext IbushakProductosContext
        {
            get { return context as IbushakProductosContext; }
        }

        public void borrarTodo()
        {
            context.Database.ExecuteSqlCommand("delete from ASIN");
        }

    }
}
