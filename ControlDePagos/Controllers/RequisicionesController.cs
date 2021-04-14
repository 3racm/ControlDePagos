using Catalogo_3R_ACM.Clases;
using ControlPagosModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControlDePagos.Controllers
{
    public class RequisicionesController : Controller
    {
        ControlPagosModel.ControlDeGastosEntities db = new ControlPagosModel.ControlDeGastosEntities();
        // GET: Requisiciones
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
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        public JsonResult ListarProyectos()
        {
            List<cRequisicion> Lista = new List<cRequisicion>();
            try
            {
                //Saul Gonzalez 14/10/2020: Obtenemos la lista de proveedores
                List<Tb_Requisiciones> tablaRequisiciones = db.Tb_Requisiciones.ToList();
                foreach (Tb_Requisiciones REQ in tablaRequisiciones)
                {
                    cRequisicion o = new cRequisicion();
                    o.Id = REQ.Id;
                    o.Cuenta_Cargo = REQ.Cuenta_Cargo;
                    o.Total = REQ.Total;     
                    o.Moneda = REQ.Moneda;
                    o.Descripcion = REQ.Descripcion;
                    o.Solicitud = REQ.Solicitud;
                    o.TipoReq = REQ.TipoReq;
                    o.FechaRegistro = REQ.FechaRegistro;
                    Lista.Add(o);
                }
            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            //Lista = Lista.OrderByDescending(x => x.FechaInicio).Reverse().ToList();
            return Json(Lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RegistrarRequisicion(string CuentaCargo, string Total, string Moneda, string Descripcion, string Solicitud, string TipoReq)
        {
            Tb_Requisiciones REQ = new Tb_Requisiciones();
            CultureInfo Culture = new CultureInfo("en-US");  //Definimos la cultura para que el separador de decimal sea por un Punto (.)    
            try
            {
                //Saul González 13/04/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(CuentaCargo))
                {
                    return Json(new { status = false, mensaje = "Debe ingresar un número de cuenta." });
                }              
                if (Convert.ToDecimal(Total, Culture) < 1)
                {
                    return Json(new { status = false, mensaje = "El monto total debe ser mayor a 0." });
                }
                if (String.IsNullOrEmpty(Moneda))
                {
                    return Json(new { status = false, mensaje = "Debe establecer una moneda para la requisición." });
                }

                if (String.IsNullOrEmpty(Descripcion))
                {
                    return Json(new { status = false, mensaje = "El campo descripción es obligatorio." });
                }
                if (String.IsNullOrEmpty(TipoReq))
                {
                    return Json(new { status = false, mensaje = "Debe indicar a quien pertenece la requisición." });
                }             
                //Saul gonzalez 13/04/2021: Asignamos valores al objeto de REQ             
                REQ.Cuenta_Cargo = CuentaCargo;
                REQ.Total = Convert.ToDecimal(Total, Culture);
                REQ.Moneda = Moneda;
                REQ.Descripcion = REQ.Descripcion;
                REQ.Solicitud = Solicitud;
                REQ.TipoReq = TipoReq;
                REQ.FechaRegistro = DateTime.Now; 
                //Saul gonzalez 13/04/2021: Guardamos los datos
                db.Tb_Requisiciones.Add(REQ);
                db.SaveChanges();
            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Requisición registrada correctamente."});
        }

        //Metodo para actualizar la requisicion.
        public JsonResult ActualizarMontoInicial(int IdReq,string CuentaCargo, string Total, string Moneda, string Descripcion, string Solicitud, string TipoReq)
        {
            CultureInfo Culture = new CultureInfo("en-US");  //Definimos la cultura para que el separador de decimal sea por un Punto (.)    
            try
            {

                //Saul González 13/04/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(IdReq.ToString()))
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al obtener el ID de la requisición." });
                }
                if (String.IsNullOrEmpty(CuentaCargo))
                {
                    return Json(new { status = false, mensaje = "Debe ingresar un número de cuenta." });
                }
                if (Convert.ToDecimal(Total, Culture) < 1)
                {
                    return Json(new { status = false, mensaje = "El monto total debe ser mayor a 0." });
                }
                if (String.IsNullOrEmpty(Moneda))
                {
                    return Json(new { status = false, mensaje = "Debe establecer una moneda para la requisición." });
                }

                if (String.IsNullOrEmpty(Descripcion))
                {
                    return Json(new { status = false, mensaje = "El campo descripción es obligatorio." });
                }
                if (String.IsNullOrEmpty(TipoReq))
                {
                    return Json(new { status = false, mensaje = "Debe indicar a quien pertenece la requisición." });
                }
                //Saul Gonzalez 23/03/2021: Consultamos los datos del proyecto
                Tb_Requisiciones Requisicion = db.Tb_Requisiciones.Where(y => y.Id == IdReq).FirstOrDefault();
                if (Requisicion == null)
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al buscar la requisición con el ID: " + IdReq.ToString() });
                }
                //Saul gonzalez 13/04/2021: Actualizamos el monto inicial del proyecto
                Requisicion.Cuenta_Cargo = CuentaCargo;
                Requisicion.Total = Convert.ToDecimal(Total, Culture);
                Requisicion.Moneda = Moneda;
                Requisicion.Descripcion = Descripcion;
                Requisicion.Solicitud = Solicitud;
                Requisicion.TipoReq = TipoReq;

                //Saul gonzalez 13/04/2021: Guardamos los datos
                db.Tb_Requisiciones.Attach(Requisicion);
                db.Entry(Requisicion).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Datos actualizados" });
        }

        public JsonResult CargarRequisicion(int IdReq)
        {
            cRequisicion REQ = new cRequisicion();    
            try
            {
                //Saul González 11/03/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(IdReq.ToString()))
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al obtener el ID de la requisición." });
                }
                //Saul Gonzalez 22/03/2021: Cargamos los datos del pago
                Tb_Requisiciones o = db.Tb_Requisiciones.Where(y => y.Id == IdReq).FirstOrDefault();
                if (o == null)
                {
                    return Json(new { status = false, mensaje = "ERROR L-194: Ocurrió un error al encontrar los datos de la requisición " + IdReq.ToString()});
                }
                REQ.Id = o.Id;
                REQ.Cuenta_Cargo = o.Cuenta_Cargo;
                REQ.Total = o.Total;
                REQ.Moneda = o.Moneda;
                REQ.Descripcion = o.Descripcion;
                REQ.Solicitud = o.Solicitud;
                REQ.TipoReq = o.TipoReq;
                REQ.FechaRegistro = o.FechaRegistro;

            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Requisición cargada.", Datos = REQ });
        }
        public JsonResult EliminarRequisicion(int IdReq)
        {
            try
            {
                //Saul González 11/03/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(IdReq.ToString()))
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al obtener el Id de la requisición." });
                }
                //Saul Gonzalez 22/03/2021: Cargamos los datos del pago
                Tb_Requisiciones REQ = db.Tb_Requisiciones.Where(y => y.Id == IdReq).FirstOrDefault();
                if (REQ == null)
                {
                    return Json(new { status = false, mensaje = "ERROR L-135: Ocurrió un error al encontrar los datos de la requisición " + IdReq.ToString()});
                }
                db.Tb_Requisiciones.Remove(REQ);
                db.SaveChanges();

            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Requisición eliminada correctamente" });
        }
    }
}