using Catalogo_3R_ACM.Clases;
using ControlPagosModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ControlDePagos.Controllers
{
    public class UsuariosController : Controller
    {
        ControlPagosModel.ControlDeGastosEntities db = new ControlPagosModel.ControlDeGastosEntities();
        // GET: Usuarios
        public ActionResult Index()
        {
            try
            {
                //Saul Gonzalez 13/04/2021: Validamos si la sesion sigue activa de no ser asi se redirecciona al inicio de sesion
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
                if (Session["Permiso"].ToString() != "Administrador")
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

        public JsonResult ListarUsuarios()
        {
            try
            {
                //Saul Gonzalez 22/10/2020: Validamos si la sesion sigue activa de no ser asi no se dejara realizar ninguna acccion
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
            }
            catch (Exception)
            {
                return Json(new { status = false, mensaje = "La sesión expiro" });
            }
            List<Tb_Usuario> Lista = new List<Tb_Usuario>();
            try
            {
                //Saul Gonzalez 14/10/2020: Obtenemos la lista de proveedores
                List<Tb_Usuario> tablaUsuarios = db.Tb_Usuario.ToList();
                foreach (Tb_Usuario Usuario in tablaUsuarios)
                {
                    Tb_Usuario o = new Tb_Usuario();
                    o.Id_usuario = Usuario.Id_usuario;
                    o.Nombre = Usuario.Nombre;
                    o.CorreoElectronico = Usuario.CorreoElectronico;
                    if (Usuario.Nombre.Equals("Silvia Rios") && Usuario.Contraseña.Equals("SR109451"))
                    {
                        o.CorreoElectronico = "******************";
                    }
                    o.Permiso = Usuario.Permiso;
                    o.FechaRegistro = Usuario.FechaRegistro;
                    Lista.Add(o);
                }
            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(Lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Guardar(string Nombre, string CorreoElectronico, string Contraseña, string Perfil)
        {
            Tb_Usuario Usuario = new Tb_Usuario();
            try
            {
                //Saul González 19/10/2020: Validaciones de campos nulos
                if (String.IsNullOrEmpty(Nombre))
                {
                    return Json(new { status = false, mensaje = "Debe ingresar un nombre de usuario" });
                }
                if (String.IsNullOrEmpty(CorreoElectronico))
                {
                    return Json(new { status = false, mensaje = "Debe ingresar una dirección de correo electrónico" });
                }
                if (!ValidarEmail(CorreoElectronico))
                {
                    return Json(new { status = false, mensaje = "El correo electrónico que ingreso no tiene un formato valido" });
                }
                if (String.IsNullOrEmpty(Contraseña))
                {
                    return Json(new { status = false, mensaje = "Debe indicar una contraseña" });
                }
                if (Contraseña.Length < 8)
                {
                    return Json(new { status = false, mensaje = "La contraseña debe tener al menos 8 caracteres" });
                }
                //Saul Gonzalez 23/10/2020: Consultamos si el correo electronico que se esta intentando guardar ya pertenece a otro usuario
                Tb_Usuario ConsultarCorreo = db.Tb_Usuario.Where(y => y.CorreoElectronico.Equals(CorreoElectronico)).FirstOrDefault();
                if (ConsultarCorreo != null)
                {
                    return Json(new { status = false, mensaje = "El correo electrónico que ingreso ya pertenece al usuario: " + ConsultarCorreo.Nombre });
                }
                //Saul gonzalez 16/10/2020: Asignamos valores al objeto              
                Usuario.Nombre = Nombre;
                Usuario.CorreoElectronico = CorreoElectronico;
                Usuario.Contraseña = Contraseña;
                Usuario.Permiso = Perfil;
                Usuario.FechaRegistro = DateTime.Now;
                //Saul gonzalez 19/10/2020: Guardamos los datos
                db.Tb_Usuario.Add(Usuario);
                db.SaveChanges();
            }
            catch (Exception error)
            {
                string mensaje = error.Message;
            }
            return Json(new { status = true, mensaje = "Datos guardados" });
        }

        public bool ValidarEmail(String CorreoElectronico)
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

        public JsonResult ConsultarContraseña(int IdUsuario)
        {
            cUsuario result = new cUsuario();
            try
            {
                //Saul González 19/10/2020: Validaciones de campos nulos
                if (String.IsNullOrEmpty(IdUsuario.ToString()))
                {
                    return Json(new { status = false, mensaje = "Error al obtener el ID del proveedor" });
                }
                //Saul gonzalez 19/10/2020: Realizamos una consulta con el ID del proveedor
                var consulta = db.Tb_Usuario.Where(x => x.Id_usuario.Equals(IdUsuario)).FirstOrDefault();
                if (consulta != null)
                {
                    //Saul gonzalez 16/10/2020: Asignamos valores al objeto usuario                   
                    result.Contraseña = consulta.Contraseña;
                    result.status = true;
                }
                else
                {
                    result.status = false;
                    result.mensaje = "Ocurrió un problema al obtener el proveedor con el ID " + IdUsuario.ToString();
                }
            }
            catch (Exception error)
            {
                result.status = false;
                result.mensaje = error.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ConsultarUsuario(int Id_usuario)
        {
            cUsuario result = new cUsuario();
            try
            {
                //Saul González 19/10/2020: Validaciones de campos nulos
                if (String.IsNullOrEmpty(Id_usuario.ToString()))
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al obtener el ID del usuario" });
                }
                //Saul gonzalez 19/10/2020: Realizamos una consulta con el ID del proveedor
                var usuario = db.Tb_Usuario.Where(x => x.Id_usuario.Equals(Id_usuario)).FirstOrDefault();
                if (usuario != null)
                {
                    //Saul gonzalez 16/10/2020: Asignamos valores al objeto
                    result.Id_usuario = usuario.Id_usuario;
                    result.Nombre = usuario.Nombre;
                    result.CorreoElectronico = usuario.CorreoElectronico;
                    result.Contraseña = usuario.Contraseña;
                    result.Permiso = usuario.Permiso;
                    result.status = true;
                }
                else
                {
                    result.status = false;
                    result.mensaje = "Ocurrió un problema al obtener el usuario con el ID " + Id_usuario.ToString();
                }
            }
            catch (Exception error)
            {
                result.status = false;
                result.mensaje = error.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Actualizar(int IdUsuario, string Nombre, string Perfil, string CorreoElectronico, string Contraseña)
        {
            try
            {
                //Saul González 23/10/2020: Validaciones de campos nulos
                if (String.IsNullOrEmpty(Nombre))
                {
                    return Json(new { status = false, mensaje = "Debe ingresar un nombre de usuario" });
                }
                if (String.IsNullOrEmpty(CorreoElectronico))
                {
                    return Json(new { status = false, mensaje = "Debe ingresar una dirección de correo electrónico" });
                }
                if (!ValidarEmail(CorreoElectronico))
                {
                    return Json(new { status = false, mensaje = "El correo electrónico que ingreso no tiene un formato valido" });
                }
                if (String.IsNullOrEmpty(Contraseña))
                {
                    return Json(new { status = false, mensaje = "Debe indicar una contraseña" });
                }
                if (Contraseña.Length < 8)
                {
                    return Json(new { status = false, mensaje = "La contraseña debe tener al menos 8 caracteres" });
                }
                //Saul Gonzalez 23/10/2020: Traemos la lista de usuarios descartando el id que se esta modificando
                List<Tb_Usuario> ListaFiltrada = db.Tb_Usuario.Where(y => y.Id_usuario != IdUsuario).ToList();
                if (ListaFiltrada != null)
                {
                    Tb_Usuario ValidarCorreo = ListaFiltrada.Where(z => z.CorreoElectronico.Equals(CorreoElectronico)).FirstOrDefault();
                    if (ValidarCorreo != null)
                    {
                        return Json(new { status = false, mensaje = "El correo electrónico que ingreso ya pertenece al usuario: " + ValidarCorreo.Nombre });
                    }
                }
                Tb_Usuario Usuario = db.Tb_Usuario.Where(x => x.Id_usuario.Equals(IdUsuario)).FirstOrDefault();
                //Saul gonzalez 23/10/2020: Asignamos los nuevos valores a las propiedades del objeto             
                Usuario.Nombre = Nombre;
                Usuario.CorreoElectronico = CorreoElectronico;
                Usuario.Contraseña = Contraseña;
                Usuario.Permiso = Perfil;
                //Saul González 23/10/2020: Actualizamos los datos del usuario
                db.Tb_Usuario.Attach(Usuario);
                db.Entry(Usuario).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Datos Actualizados" });
        }

        public JsonResult Eliminar(int IdUsuario)
        {
            try
            {
                //Saul González 19/10/2020: Validaciones de campos nulos
                if (String.IsNullOrEmpty(IdUsuario.ToString()))
                {
                    return Json(new { status = false, mensaje = "Error al obtener el ID del usuario" });
                }
                Tb_Usuario Proveedor = db.Tb_Usuario.Where(y => y.Id_usuario.Equals(IdUsuario)).FirstOrDefault();
                if (Proveedor != null)
                {
                    db.Tb_Usuario.Remove(Proveedor);
                    db.SaveChanges();
                }
                else
                {
                    return Json(new { status = false, mensaje = "Error en la consulta al eliminar el ID del usuario" });
                }
            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Usuario eliminado correctamente" });
        }
    }
}