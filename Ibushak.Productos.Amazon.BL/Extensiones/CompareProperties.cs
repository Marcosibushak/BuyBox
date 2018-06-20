using System;
using Ibushak.Productos.Amazon.BL.Model;
using System.Collections.Generic;
using System.Reflection;

namespace Ibushak.Productos.Amazon.BL.Extensiones
{
    internal static class CompareProperties
    {
        public static List<CampoActualizado> DetailedCompare<T>(this T val1, T val2)
        {
            List<CampoActualizado> variantes = new List<CampoActualizado>();
            FieldInfo[] fi = val1.GetType().GetFields(BindingFlags.Instance |
                       BindingFlags.Static |
                       BindingFlags.NonPublic |
                       BindingFlags.Public);
            foreach (FieldInfo f in fi)
            {
                if (!f.Name.Equals("<Actualizacion>k__BackingField"))
                {
                    if (f.FieldType.Name == "String" || f.FieldType.Name == "Int64" || f.FieldType.Name == "Boolean" || f.FieldType.Name == "Int32" ||
                    f.FieldType.Name == "Decimal")
                    {
                        try
                        {
                            var prod = new CampoActualizado
                            {
                                Campo = f.Name,
                                ValorNuevo = f.GetValue(val1) != null ? f.GetValue(val1) : null,
                                ValorViejo = val2 != null ? f.GetValue(val2) : null
                            };
                            if (prod.ValorNuevo != null && prod.ValorViejo != null)
                            {
                                if (!prod.ValorNuevo.Equals(prod.ValorViejo))
                                    variantes.Add(prod);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"-----------------El valor con error es: \nvalor1: {val1}\nvalor2: {val2}\n\n");
                            Console.WriteLine(ex);
                        }
                    }
                }
            }
            return variantes;
        }
    }
}