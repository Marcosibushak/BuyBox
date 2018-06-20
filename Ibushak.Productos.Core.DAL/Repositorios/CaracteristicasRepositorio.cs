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
    public class CaracteristicasRepositorio : Repositorio<Caracteristicas>, ICaracteristicasRepositorio
    {
        public CaracteristicasRepositorio(IbushakProductosContext context) : base(context)
        {
        }

        public IbushakProductosContext IbushakProductosContext
        {
            get { return context as IbushakProductosContext; }
        }

        public void borrarTodo(string id)
        {
            string sqlQuery = "DELETE FROM Caracteristicas WHERE ASIN = @id";

            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("@id", id));

            context.Database.ExecuteSqlCommand(sqlQuery, parameterList.ToArray());
        }

        public IEnumerable<Caracteristicas> obtenerCaracteristicasActualizados(IEnumerable<string> asin)
        {
            var resultado = IbushakProductosContext.Caracteristicas.Where(caracteristica =>
                asin.Contains(caracteristica.ASIN)).AsEnumerable();

            return resultado;
        }
    }
}
