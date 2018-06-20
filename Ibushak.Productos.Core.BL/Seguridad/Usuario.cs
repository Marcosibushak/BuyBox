using Ibushak.Productos.Core.BL.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ibushak.Productos.Core.BL.Seguridad
{
    public class Usuario
    {
        public bool autenticarUsuario(string usuario, string contrasenia)
        {
            if (UsuariosAdapter.existe(usuario))
            {
                if (UsuariosAdapter.validarUsuario(usuario, contrasenia))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
    }
}
