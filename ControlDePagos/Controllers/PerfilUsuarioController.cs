using Catalogo_3R_ACM.Clases;
using ControlPagosModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControlDePagos.Controllers
{
    public class PerfilUsuarioController : Controller
    {
        ControlPagosModel.ControlDeGastosEntities db = new ControlPagosModel.ControlDeGastosEntities();
        // GET: PerfilUsuario
        public ActionResult Index()
        {
            try
            {
                //Saul Gonzalez 27/04/2021: Validamos si la sesion sigue activa de no ser asi no se dejara realizar ninguna acccion
                var Usuario = Session["Usuario"].ToString();
                string[] Nombre = Usuario.Split(' ');
                if (Nombre.Length > 1)
                {
                    ViewBag.Usuario = Nombre[0] + " " + Nombre[1];
                }
                else
                {
                    ViewBag.Usuario = Nombre[0];
                }
                if (String.IsNullOrEmpty(Session["Usuario"].ToString()))
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        public JsonResult Cargar()
        {

            cUsuario result = new cUsuario();
            try
            {
                string CorreoElectronico = Session["CorreoElectronico"].ToString();
                //Saul González 19/10/2020: Validaciones de campos nulos
                if (String.IsNullOrEmpty(CorreoElectronico.ToString()))
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al obtener los datos del usuario" });
                }
                //Saul gonzalez 19/10/2020: Realizamos una consulta con el ID del proveedor
                var usuario = db.Tb_Usuario.Where(x => x.CorreoElectronico.Equals(CorreoElectronico)).FirstOrDefault();
                if (usuario != null)
                {
                    //Saul gonzalez 16/10/2020: Asignamos valores al objeto
                    result.Id_usuario = usuario.Id_usuario;
                    result.Nombre = usuario.Nombre;
                    result.CorreoElectronico = usuario.CorreoElectronico;
                    result.Contraseña = usuario.Contraseña;
                    result.Permiso = usuario.Permiso;
                    result.FechaTexto = usuario.FechaRegistro.ToShortDateString();
                    result.status = true;
                }
                else
                {
                    result.status = false;
                    result.mensaje = "Ocurrió un problema al obtener el usuario con el correo " + CorreoElectronico.ToString();
                }
            }
            catch (Exception error)
            {
                result.status = false;
                result.mensaje = error.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ActualizarDatos(int IdUsuario, string Contraseña, string ContraseñaConfirmada)
        {
            try
            {
                if (String.IsNullOrEmpty(Contraseña.ToString()) && String.IsNullOrEmpty(ContraseñaConfirmada))
                {
                    return Json(new { status = false, mensaje = "No se realizó ningún cambio" });
                }
                if (String.IsNullOrEmpty(IdUsuario.ToString()))
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al obtener el ID del usuario" });
                }
                if (String.IsNullOrEmpty(Contraseña.ToString()))
                {
                    return Json(new { status = false, mensaje = "Debe ingresar una contraseña" });
                }
                if (String.IsNullOrEmpty(ContraseñaConfirmada.ToString()))
                {
                    return Json(new { status = false, mensaje = "Por motivos de seguridad, debe confirmar 2 veces la contraseña" });
                }
                if (Contraseña != ContraseñaConfirmada)
                {
                    return Json(new { status = false, mensaje = "Las contraseñas no coinciden" });
                }
                if (Contraseña.Length < 8)
                {
                    return Json(new { status = false, mensaje = "La contraseña debe tener al menos 8 caracteres" });
                }
                Tb_Usuario Usuario = db.Tb_Usuario.Where(y => y.Id_usuario.Equals(IdUsuario)).FirstOrDefault();
                if (Usuario != null)
                {
                    Usuario.Contraseña = ContraseñaConfirmada;
                }
                else
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al obtener el los datos del usuario" });
                }
                //Saul González 28/10/2020: Actualizamos los datos del usuario
                db.Tb_Usuario.Attach(Usuario);
                db.Entry(Usuario).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { status = true, mensaje = "Contraseña actualizada" });
            }
            catch (Exception error)
            {

                return Json(new { status = false, mensaje = error.Message });
            }

        }
    }
}