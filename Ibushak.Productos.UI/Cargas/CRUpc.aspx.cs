using Ibushak.Productos.Core.BL.Archivos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Ibushak.Productos.UI.Cargas
{
    public partial class CRUpc : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var user = Session["USUARIO"];
            if (string.IsNullOrEmpty(user?.ToString()))
            {
                Response.Redirect("~/Default.aspx");
            }
        }

        protected void bstUploadControlUpc_FileUploadComplete(object sender, EventArgs e)
        {
            if (!bstUploadControlUpc.HasFile) return;
            try
            {
                labelResUpload.Text = GuardarArchivo(bstUploadControlUpc.PostedFiles);
                var script = "UploadComplete('"+ bstUploadControlUpc.PostedFile.FileName+"');";
                ScriptManager.RegisterStartupScript(this, GetType(), "testScript", script, true);
            }
            catch (Exception ex)
            {
                labelResUpload.Text = ex.Message;
            }
        }

        protected void cbp_proceso_Callback(object sender, EventArgs e)
        {
            var oCarga = new Carga();

            var Resultado = false;

            Resultado = oCarga.CargarAsinUpcs(Session["RutaArchivo"].ToString());
            if (Resultado)
            {
                lbl_mensaje.ForeColor = System.Drawing.Color.Blue;
                lbl_mensaje.Text = "La Actualización del Archivo concluyó con Exito.";
            }
            else
            {
                lbl_mensaje.ForeColor = System.Drawing.Color.Red;
                lbl_mensaje.Text = oCarga.ErrorMensaje;
            }

            oCarga = null;
        }

        //[WebMethod]
        //public void CargarArchivo()
        //{
        //    var oCarga = new Carga();

        //    var Resultado = false;

        //    Resultado = oCarga.CargarAsinUpcs(Session["RutaArchivo"].ToString());
        //    if (Resultado)
        //    {
        //        lbl_mensaje.ForeColor = System.Drawing.Color.Blue;
        //        lbl_mensaje.Text = "La Actualización del Archivo concluyó con Exito.";
        //    }
        //    else
        //    {
        //        lbl_mensaje.ForeColor = System.Drawing.Color.Red;
        //        lbl_mensaje.Text = oCarga.ErrorMensaje;
        //    }

        //    oCarga = null;
        //}

        private string GuardarArchivo(IList<HttpPostedFile> archivos)
        {
            var ret = "";
            if (!archivos.Any()) return ret;
            foreach (var archivo in archivos)
            {
                var filename = archivo.FileName;
                if (!filename.Contains(".xlsx"))
                    filename += "x";
                var url = $"~/Archivos/{filename}";
                var ruta = Server.MapPath(url);

                archivo.SaveAs(ruta);
                Session["RutaArchivo"] = ruta;

                var fileLabel = archivo.FileName;

                var fileSize = (int)archivo.ContentLength / 1024;
                var fileLength = fileSize.ToString("N0") + " KB";
                ret += $"<font color='blue'>{fileLabel}</font> <i>({fileLength})</i> </br>";
            }
            var oCarga = new Carga();

            var resultado = oCarga.CargarAsinUpcs(Session["RutaArchivo"].ToString());
            if (resultado)
            {
                lbl_mensaje.ForeColor = System.Drawing.Color.Blue;
                lbl_mensaje.Text = $"La Actualización del Archivo concluyó con Exito.\n{oCarga.MessageCount}";
            }
            else
            {
                lbl_mensaje.ForeColor = System.Drawing.Color.Red;
                lbl_mensaje.Text = oCarga.ErrorMensaje;
            }

            ret += "OK";
            return ret;
        }
    }
}