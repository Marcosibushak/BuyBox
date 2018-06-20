using System;

namespace Ibushak.Productos.UI.Catalogos
{
    public partial class CAUpc : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var user = Session["USUARIO"];
            if (string.IsNullOrEmpty(user?.ToString()))
            {
                Response.Redirect("~/Default.aspx");
            }
        }
    }
}