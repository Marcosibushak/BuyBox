using Ibushak.Productos.Core.DomainModel.Catologos;
using Ibushak.Productos.Core.DomainModel.Interfaces.Repositorios;
using System.Collections.Generic;
using System.Linq;

namespace Ibushak.Productos.Core.DAL.Repositorios
{
    public class ProductoRepositorio : Repositorio<Producto>, IProductoRepositorio
    {
        public ProductoRepositorio(IbushakProductosContext context) : base(context)
        {
        }

        public IbushakProductosContext IbushakProductosContext
        {
            get { return context as IbushakProductosContext; }
        }

        public IEnumerable<Producto> obtenerProductosActualizados()
        {
            var resultado = (from producto in IbushakProductosContext.Productos
                            where producto.Actualizacion == true
                            select producto).AsEnumerable();

            return resultado;
        }

        public void DeleteAll()
        {
            context.Database.ExecuteSqlCommand("delete from Resumen");
            context.Database.ExecuteSqlCommand("delete from DimensionesPaquete");
            context.Database.ExecuteSqlCommand("delete from Dimensiones");
            context.Database.ExecuteSqlCommand("delete from Comentarios");
            context.Database.ExecuteSqlCommand("delete from Producto");
        }
    }
}