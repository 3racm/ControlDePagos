using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ControlPagosModel;

namespace ControlDePagos.Controllers
{
    public class LoginController : Controller
    {
        ControlPagosModel.ControlDeGastosEntities db = new ControlPagosModel.ControlDeGastosEntities();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ValidarUsuario(string Usuario, string Contraseña)
        {
            try
            {         
                //Saul González 09/03/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(Usuario))
                {
                    return Json(new { status = false, mensaje = "El correo electrónico no puede estar vacío" });
                }              
                //Saul González 09/03/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(Contraseña))
                {
                    return Json(new { status = false, mensaje = "Debe indicar una contraseña" });
                }
                //Saul Gonzalez 09/03/2021: Consultamos si existe el usuario segun el correo y contraseña
                //FALTA VALIDAR QUE LA CONSULTA DISTINGA ENTRE MAYUSCULAS Y MINUSCULAS
                Tb_Usuario oUsuario = db.Tb_Usuario.Where(y => y.CorreoElectronico.Equals(Usuario) && y.Contraseña.Equals(Contraseña)).FirstOrDefault();
                if (oUsuario == null)
                {
                    return Json(new { status = false, mensaje = "El correo electrónico o contraseña que ingreso no son correctos, verifique los datos y vuelva a intentarlo." });
                }
                else
                {
                    //Saul Gonzalez 09/03/2021: Almacenamos las variables de sesion
                    Session["Usuario"] = oUsuario.Nombre;
                    Session["CorreoElectronico"] = oUsuario.CorreoElectronico;
                    Session["Contraseña"] = oUsuario.CorreoElectronico;
                    Session["Permiso"] = oUsuario.Permiso;
                    //Saul Gonzalez 09/03/2021: Establece el tiempo para destruir la sesion si no se realiza alguna peticion 
                    //Session.Timeout = 0;
                }

            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Inicio de sesion exitoso" });
        }
        public JsonResult CerrarSesion()
        {
            try
            {
                Session.Abandon();
            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Ha finalizado la sesión" });
        }

        public JsonResult EnviarCorreoDeRecuperacion(string CorreoElectronico)
        {
            try
            {
                //Saul González 23/04/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(CorreoElectronico))
                {
                    return Json(new { status = false, mensaje = "El correo electrónico no puede estar vacío" });
                }
                if (!ValidarEmail(CorreoElectronico))
                {
                    return Json(new { status = false, mensaje = "El correo electrónico que ingreso no tiene un formato valido" });
                }
                //Saul Gonzalez 23/04/2021: Consultamos si existe el usuario segun el correo y contraseña
                Tb_Usuario Usuario = db.Tb_Usuario.Where(y => y.CorreoElectronico.Equals(CorreoElectronico)).FirstOrDefault();
                if (Usuario == null)
                {
                    return Json(new { status = false, mensaje = "El correo electrónico que ingreso no se encuentra registrado." });
                }
                else
                {
                    WebClient cliente = new WebClient();
                    //Saul Gonzalez 23/04/2021: Leemos la plantilla HTML de recuperación  
                    string PlantillaRecuperacion = cliente.DownloadString(@"C:\Plantillas\RecuperacionControlDePagos.html");
                    //Saul Gonzalez 23/04/2021: Remplazamos la cadena en el correo para que muestre el nombre del usuario   
                    var PlantillaRemplazada = PlantillaRecuperacion.Replace("{Nombre}", Usuario.Nombre.ToString());
                    //Saul Gonzalez: 23/04/2021: Generamos una contraseña aleatoria
                    Random rdn = new Random();
                    string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890$#@";
                    int longitud = caracteres.Length;
                    char letra;
                    int longitudContrasenia = 8;
                    string contraseniaAleatoria = string.Empty;
                    for (int i = 0; i < longitudContrasenia; i++)
                    {
                        letra = caracteres[rdn.Next(longitud)];
                        contraseniaAleatoria += letra.ToString();
                    }
                    //Saul Gonzalez 23/04/2021: Remplazamos la cadena de la contraseña en el correo para que muestre la nueva contraseña generada 
                    PlantillaRemplazada = PlantillaRemplazada.Replace("{ContraseñaTemp}", contraseniaAleatoria.ToString());

                    //Saul Gonzalez 23/04/2021: Cambiamos la contraseña al usuario
                    Usuario.Contraseña = contraseniaAleatoria;
                    db.Tb_Usuario.Attach(Usuario);
                    db.Entry(Usuario).State = EntityState.Modified;
                    db.SaveChanges();

                    //Datos del correo como destinatario, cuerpo, asunto.
                    MailMessage msg = new MailMessage();
                    msg.From = new MailAddress("3rcatalogo@3racm.com");
                    msg.To.Add(CorreoElectronico);
                    msg.Subject = "Control de pagos 3R: Recuperación de contraseña";
                    msg.Body = PlantillaRemplazada;
                    msg.IsBodyHtml = true;
                    //msg.Priority = MailPriority.High; 

                    //Configuración y credenciales del correo origen, es decir desde este correo se mandara el mensaje de recuperación
                    using (SmtpClient client = new SmtpClient())
                    {
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential("3rcatalogo@3racm.com", "WYVmW@z6");
                        client.Host = "smtp.hostinger.mx";
                        client.Port = 587;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        //Enviamos el correo
                        client.Send(msg);
                    }
                }

            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Se envió una contraseña temporal a: " + CorreoElectronico });
        }

        private bool ValidarEmail(String CorreoElectronico)
        {
            String expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(CorreoElectronico, expresion))
            {
                if (Regex.Replace(CorreoElectronico, expresion, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}