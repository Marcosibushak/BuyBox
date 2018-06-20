using Ibushak.Productos.Core.BL.Seguridad;
using System;

namespace Ibushak.Productos.UI
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["USUARIO"] = "";
        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            var oUsuario = new Usuario();
            if(oUsuario.autenticarUsuario(txtUsuario.Text, txtContrasenia.Text))
            {
                Session["USUARIO"] = txtUsuario.Text;
                Response.Redirect("~/Cargas/CRUpc.aspx");
            }
            else
            {
                lblError.Text = "Usuario/Contraseña incorrecta";
            }
        }
    }
}