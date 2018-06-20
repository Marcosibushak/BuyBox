using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ibushak.Productos.Amazon.BL.Extensiones
{
    internal static class Colecciones
    {
        public static DataTable ConvertirADataTable<T>(this IEnumerable<T> coleccion, string nombreTabla)
        {
            var props = typeof(T).GetProperties();
            var propsFiltradas = (from prop in props
                                  where prop.PropertyType.Name == "String" || prop.PropertyType.Name == "Int64"
                                        || prop.PropertyType.Name == "Boolean" || prop.PropertyType.Name == "Int32"
                                        || prop.PropertyType.Name == "Decimal"
                                  select prop).ToArray();

            var dt = new DataTable {TableName = nombreTabla};
            dt.Columns.AddRange(propsFiltradas.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());
            coleccion.ToList().ForEach(i => dt.Rows.Add(propsFiltradas.Select(p => p.GetValue(i, null)).ToArray()));
            return dt;
        }
    }
}