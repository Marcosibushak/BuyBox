using System;
using System.Net.Mail;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;

namespace Ibushak.Productos.Core.BL.Envios
{
    public class Correo
    {
        public string ErrorText { get; private set; }

        // Preparamos datos para el envío de Correo Electronico
        private readonly string _emailFrom = ConfigurationManager.AppSettings["smtpfrom"];
        private readonly string[] _emailCc = ConfigurationManager.AppSettings["smtpCC"].Split(',');
        private readonly string[] _emailBcc = ConfigurationManager.AppSettings["smtpBCC"].Split(',');
        private readonly string[] _emailTo = ConfigurationManager.AppSettings["smtpTo"].Split(',');
        //private string[] _emailToSupport = ConfigurationManager.AppSettings["toCsupport"].Split(','); //Administradores de Aplicacion

        // Preparamos datos para el Servidor de Correo Electronico
        private readonly string _smtpServer = ConfigurationManager.AppSettings["smtp"];
        private readonly string _smtpPwd = ConfigurationManager.AppSettings["smtppwd"];
        private readonly string _smtpUser = ConfigurationManager.AppSettings["smtpuser"];

        //private static string _rutaFormatosConfig = ConfigurationManager.AppSettings["FormatosEmails"].ToString();

        public bool EnviarMensaje(byte[] archivo, string nombreArchivo)
        {
            //List<string> emailTo = new List<string>();
            //emailTo.Add(correo);

            //var _rutaFormatos = HttpContext.Current.Server.MapPath(_rutaFormatosConfig);

            //emailBody = File.ReadAllText(_rutaFormatos);
            var emailBody = "Reporte de seguimientos de Ibushak en Amazon";
            var envioOk = SendEmail(_emailFrom, _emailTo, _emailCc, _emailBcc, emailBody, "Seguimiento Amazon", nombreArchivo, archivo);

            return envioOk;
        }

        private bool SendEmail(string FromR, string[] ToS, string[] ccS, string[] bccS, string msg, string Titulo, string nombreArchivo, byte[] archivo = null)
        {
            MailMessage CorreoElectronico = new MailMessage();
            Attachment oAttach = default(Attachment);
            SmtpClient SMTPServer = new SmtpClient();

            var success = true;
            ErrorText = "OK";

            try
            {
                CorreoElectronico.From = new MailAddress(FromR);

                string[] Destinatarios = ToS;

                if (Destinatarios != null)
                {
                    foreach (string Destinatario in Destinatarios)
                    {
                        if (isValidEmail(Destinatario))
                        {
                            CorreoElectronico.To.Add(Destinatario);
                        }
                    }
                }

                CorreoElectronico.Bcc.Add("fernando.loman@triplede.net");
                CorreoElectronico.Bcc.Add("guillermo.vargas@triplede.net");
                CorreoElectronico.Bcc.Add("gerardo.miranda@triplede.net");

                string[] Copiados = ccS;

                if (Copiados != null)
                {
                    foreach (string Copiado in Copiados)
                    {
                        if (isValidEmail(Copiado))
                        {
                            CorreoElectronico.CC.Add(Copiado);
                        }
                    }
                }

                string[] Ocultos = bccS;

                if (Ocultos != null)
                {
                    foreach (string Oculto in Ocultos)
                    {
                        if (isValidEmail(Oculto))
                        {
                            CorreoElectronico.Bcc.Add(Oculto);
                        }
                    }
                }

                if (CorreoElectronico.To.Count < 1)
                {
                    throw new Exception("No hay Destinatarios validos</br> Envio de mensaje CANCELADO");
                }

                CorreoElectronico.Subject = Titulo;
                CorreoElectronico.IsBodyHtml = true;
                CorreoElectronico.Priority = MailPriority.High;

                CorreoElectronico.Body = msg.ToString();

                //Anexo en PDF.
                if (archivo != null)
                {
                    
                    oAttach = new Attachment(new MemoryStream(archivo), nombreArchivo);
                    CorreoElectronico.Attachments.Add(oAttach);
                    //if (File.Exists(PDFfile))
                    //{
                    //    oAttach = new Attachment(PDFfile);
                    //    CorreoElectronico.Attachments.Add(oAttach);
                    //}
                }


                //Asignamos el nombre del servidor SMTP
                SMTPServer.Host = _smtpServer;
                SMTPServer.Port = 587;
                SMTPServer.EnableSsl = true;
                SMTPServer.DeliveryMethod = SmtpDeliveryMethod.Network;

                //----------------------------------------------------------------------------------------------------------------------
                //var fromAddress = new MailAddress(_smtpUser, "BuyBox");
                //Create a new NetworkCredential, and set the User Name and Password
                //System.Net.NetworkCredential authenticationInfo = new System.Net.NetworkCredential(CorreoElectronico.From.Address, _smtpPwd);
                //False, because we are providing credentials
                SMTPServer.UseDefaultCredentials = false;
                SMTPServer.Credentials = new NetworkCredential(_smtpUser, _smtpPwd);
                //----------------------------------------------------------------------------------------------------------------------

                // Envia el mensaje
                SMTPServer.Send(CorreoElectronico);

            }
            catch (Exception ex)
            {
                string Inner = "-- None --";
                if (ex.InnerException != null)
                    Inner = ex.InnerException.Message;

                ErrorText = ex.Message;
                success = false;
            }
            finally
            {
                SMTPServer = null;
                oAttach = null;
                CorreoElectronico = null;
            }
            return success;
        }

        private bool isValidEmail(string strIn)
        {

            // Return true if strIn is in valid e-mail format.
            if (strIn != null)
            {
                return Regex.IsMatch(strIn, "^([\\w-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([\\w-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$");
            }
            else
            {
                return false;
            }

        }
    }
}
