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

        public JsonResult ListarRequisiciones()
        {
            List<cRequisicion> Lista = new List<cRequisicion>();
            try
            {
                //Saul Gonzalez 14/10/2020: Obtenemos la lista de proveedores
                List<Tb_Requisiciones> tablaRequisiciones = db.Tb_Requisiciones.ToList();
                foreach (Tb_Requisiciones REQ in tablaRequisiciones)
                {
                    cRequisicion o = new cRequisicion();
                    o.Folio = REQ.Folio;
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

        public JsonResult Registrar(string NoFolio, string Total, string Moneda, string CuentaCargo, string Solicitud, string TipoReq, string Descripcion)
        {
            Tb_Requisiciones REQ = new Tb_Requisiciones();
            CultureInfo Culture = new CultureInfo("en-US");  //Definimos la cultura para que el separador de decimal sea por un Punto (.)    
            try
            {
                //Saul González 15/04/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(NoFolio))
                {
                    return Json(new { status = false, mensaje = "Debe indicar un número de folio para ello seleccione el tipo de requisición." });
                }
                if (Convert.ToDecimal(Total, Culture) < 1)
                {
                    return Json(new { status = false, mensaje = "El monto total debe ser mayor a 0." });
                }
                if (String.IsNullOrEmpty(Moneda))
                {
                    return Json(new { status = false, mensaje = "Debe seleccionar una moneda para la requisición." });
                }
                if (String.IsNullOrEmpty(CuentaCargo))
                {
                    return Json(new { status = false, mensaje = "Debe ingresar un número de cuenta." });
                }
                if (String.IsNullOrEmpty(TipoReq))
                {
                    return Json(new { status = false, mensaje = "Debe indicar el tipo de requisición." });
                }
                if (String.IsNullOrEmpty(Descripcion))
                {
                    return Json(new { status = false, mensaje = "El campo descripción es obligatorio." });
                }

                //Saul gonzalez 15/04/2021: Asignamos valores al objeto de REQ             
                REQ.Folio = NoFolio;
                REQ.Cuenta_Cargo = CuentaCargo;
                REQ.Total = Convert.ToDecimal(Total, Culture);
                REQ.Moneda = Moneda;
                REQ.Descripcion = Descripcion;
                REQ.Solicitud = Solicitud;
                REQ.TipoReq = TipoReq;
                REQ.FechaRegistro = DateTime.Now;
                //Saul gonzalez 15/04/2021: Guardamos los datos
                db.Tb_Requisiciones.Add(REQ);
                db.SaveChanges();
            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Requisición registrada correctamente." });
        }

        //Metodo para actualizar la requisicion.
        public JsonResult ActualizarRequisicion(string NoFolio, string Total, string Moneda, string CuentaCargo, string Solicitud, string TipoReq, string Descripcion)
        {
            CultureInfo Culture = new CultureInfo("en-US");  //Definimos la cultura para que el separador de decimal sea por un Punto (.)    
            try
            {

                //Saul González 13/04/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(NoFolio.ToString()))
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
                Tb_Requisiciones Requisicion = db.Tb_Requisiciones.Where(y => y.Folio == NoFolio).FirstOrDefault();
                if (Requisicion == null)
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al buscar la requisición con el ID: " + NoFolio.ToString() });
                }
                //Saul gonzalez 13/04/2021: Actualizamos el monto inicial del proyecto
                Requisicion.Folio = NoFolio;
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

        public JsonResult CargarRequisicion(string IdRequisicion, string TipoReq)
        {
            cRequisicion REQ = new cRequisicion();
            try
            {
                //Saul González 15/04/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(IdRequisicion.ToString()))
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al obtener el ID de la requisición." });
                }
                if (String.IsNullOrEmpty(TipoReq))
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al obtener el tipo de requisición." });
                }
                //Saul Gonzalez 16/04/2021: Cargamos los datos del pago
                Tb_Requisiciones o = db.Tb_Requisiciones.Where(y => y.Folio == IdRequisicion.ToString() && y.TipoReq == TipoReq).FirstOrDefault();
                if (o == null)
                {
                    return Json(new { status = false, mensaje = "ERROR L-194: Ocurrió un error al encontrar los datos de la requisición " + IdRequisicion.ToString() });
                }
                REQ.Folio = o.Folio;
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
        public JsonResult EliminarRequisicion(string IdReq)
        {
            try
            {
                //Saul González 16/04/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(IdReq.ToString()))
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al obtener el Id de la requisición." });
                }
                //Saul Gonzalez 16/04/2021: Cargamos los datos del pago
                Tb_Requisiciones REQ = db.Tb_Requisiciones.Where(y => y.Folio == IdReq.ToString()).FirstOrDefault();

                if (REQ == null)
                {
                    return Json(new { status = false, mensaje = "ERROR L-1231: Ocurrió un error al encontrar y cancelar los datos de la requisición " + IdReq.ToString() });
                }
                //Realizamos un split para cambiar el codigo de la requisicion por cancelada pero conservando el consecutivo
                var Cadena = REQ.Folio.Split('-');
                string NuevoFolio = "CANCELADA-" + Cadena[1].ToString();
                REQ.Folio = NuevoFolio;

                //Saul gonzalez 16/04/2021: Guardamos los datos
                db.Tb_Requisiciones.Attach(REQ);
                db.Entry(REQ).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Requisición cancelada correctamente" });
        }

        public JsonResult GenerarNumeroDeFolio(string TipoReq)
        {
            var Folio = "";
            string AñoActual = DateTime.Now.Year.ToString();
            try
            {
                //Saul González 15/04/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(TipoReq))
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al leer el valor de la requisición" });
                }
                //Saul Gonzalez 15/04/2021: Obtenemos el ultimo elemento registrado
                List<Tb_Requisiciones> Lista = db.Tb_Requisiciones.ToList();
                if (Lista.Count > 0)
                {
                    var RegistroReq = Lista.LastOrDefault();

                    var cadena = RegistroReq.Folio.Split('-');
                    var secuencia = cadena[1];
                    var numeracion = Convert.ToInt32(secuencia);
                    numeracion = numeracion + 1;


                    //Segun el numero de registro que exista creamos la numeracion para el folio
                    if (numeracion < 10)
                    {
                        Folio = "3R" + AñoActual + "-00" + numeracion.ToString();
                    }
                    if (numeracion > 9 && numeracion < 100)
                    {
                        Folio = "3R" + AñoActual + "-0" + numeracion.ToString();
                    }
                    if (numeracion > 99)
                    {
                        Folio = "3R" + AñoActual + "-" + numeracion.ToString();
                    }
                }
                else
                {
                    //Si la tabla esta vacia asignamos el numero 001                  
                    Folio = "3R" + AñoActual + "-001";
                }

            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Folio generado correctamente.", Folio = Folio });
        }
    }
}