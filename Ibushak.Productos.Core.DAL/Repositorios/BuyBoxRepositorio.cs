using Ibushak.Productos.Core.DomainModel.Interfaces.Repositorios;
using Ibushak.Productos.Core.DomainModel.Ofertas;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Ibushak.Productos.Core.DAL.Repositorios
{
    public class BuyBoxRepositorio : Repositorio<BuyBox>, IBuyBoxRepositorio
    {
        public BuyBoxRepositorio(IbushakProductosContext context) : base(context)
        {
        }
        
        public IbushakProductosContext IbushakProductosContext
        {
            get { return context as IbushakProductosContext; }
        }

        public BuyBox Obtener(string id, string condicion)
        {
            return context.Set<BuyBox>().Find(id, condicion);
        }

        public List<BuyBox> ObtenerList(string asin)
        {
            return context.Set<BuyBox>().Where(x => x.ASIN == asin).ToList();
        }

        public void BorrarTodo(string id)
        {
            string sqlQuery = "DELETE FROM BuyBox WHERE ASIN = @id";

            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@id", id));

            context.Database.ExecuteSqlCommand(sqlQuery, parameterList.ToArray());
        }

        public IEnumerable<BuyBox> ObtenerBuyBoxActualizados(IEnumerable<string> asin)
        {
            var resultado = (from buyBox in IbushakProductosContext.BuyBox
                             where asin.Contains(buyBox.ASIN)
                             select buyBox).AsEnumerable();

            return resultado;
        }
    }
}
