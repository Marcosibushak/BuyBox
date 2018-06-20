using Ibushak.Productos.Core.DomainModel.Datos;
using Ibushak.Productos.Core.DomainModel.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ibushak.Productos.Core.DAL.Repositorios
{
    public class UPCsRepositorio : Repositorio<UPCs>, IUPCsRepositorio
    {
        public UPCsRepositorio(IbushakProductosContext context) : base(context)
        {
        }

        public IbushakProductosContext IbushakProductosContext
        {
            get { return context as IbushakProductosContext; }
        }

        public void borrarTodo(string id)
        {
            string sqlQuery = "DELETE FROM UPCs WHERE ASIN = @id";

            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@id", id));

            context.Database.ExecuteSqlCommand(sqlQuery, parameterList.ToArray());
        }

        public IEnumerable<UPCs> obtenerUPCsActualizados(IEnumerable<string> asin)
        {
            var resultado = (from upc in IbushakProductosContext.UPCs
                             where asin.Contains(upc.ASIN)
                             select upc).AsEnumerable();

            return resultado;
        }
    }
}
