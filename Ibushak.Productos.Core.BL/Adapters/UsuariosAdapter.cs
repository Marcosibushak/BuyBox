using Ibushak.Productos.Core.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ibushak.Productos.Core.BL.Adapters
{
    public static class UsuariosAdapter
    {
        public static bool existe (string usuario)
        {
            bool existe = false;

            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
            {
                int cont = unidadDeTrabajo.Usuario.buscar(u => u.Id == usuario).AsQueryable().Count();

                existe = cont > 0 ? true : false;
            }

            return existe;
        }

        public static bool validarUsuario(string usuario, string contrasenia)
        {
            bool existe = false;

            using (var unidadDeTrabajo = new UnidadDeTrabajo(new IbushakProductosContext()))
            {
                int cont = unidadDeTrabajo.Usuario.buscar(u => u.Id == usuario && u.Psw == contrasenia).AsQueryable().Count();

                existe = cont > 0 ? true : false;
            }

            return existe;
        }
    }
}
