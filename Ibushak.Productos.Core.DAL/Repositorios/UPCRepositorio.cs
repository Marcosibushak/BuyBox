using Ibushak.Productos.Core.DomainModel.Catologos;
using Ibushak.Productos.Core.DomainModel.Interfaces.Repositorios;

namespace Ibushak.Productos.Core.DAL.Repositorios
{
    public class UPCRepositorio : Repositorio<UPC>, IUPCRepositorio
    {
        public UPCRepositorio(IbushakProductosContext context) : base(context)
        {
        }

        public IbushakProductosContext IbushakProductosContext
        {
            get { return context as IbushakProductosContext; }
        }

        public void borrarTodo()
        {
            context.Database.ExecuteSqlCommand("delete from UPC");
        }

    }
}