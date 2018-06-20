using System;
using System.Web.UI;

namespace Ibushak.Productos.UI.Catalogos
{
    public partial class CAAsin : Page
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