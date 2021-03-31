using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}