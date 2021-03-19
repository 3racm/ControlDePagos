using Catalogo_3R_ACM.Clases;
using ControlPagosModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControlDePagos.Controllers
{
    public class PagosController : Controller
    {
        ControlPagosModel.ControlDeGastosEntities db = new ControlPagosModel.ControlDeGastosEntities();
        // GET: Pagos
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CargarPagina(int IdProyecto = 0)
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
                if (String.IsNullOrEmpty(Session["Usuario"].ToString()))
                {
                    LoginController Login = new LoginController();
                    return View();
                }
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Login");
            }
            return View("Index");
        }

        public JsonResult CargarDatosProyecto(int IdProyecto = 0)
        {
            cProyectos DatosProyecto = new cProyectos();
            List<cPagos> ListaPagos = new List<cPagos>();
            //Variables que calculan el subtotal, totalRetorno y MontoFinal
            decimal Subtotal = 0;
            decimal ? TotalRetorno = 0;
            decimal ? MontoFinal = 0;
            try
            {
                //Saul González 12/03/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(IdProyecto.ToString()))
                {
                    return Json(new { status = false, mensaje = "Error L52: Ocurrio un error al obtener el Id del proyecto." });
                }
                //Saul González 12/03/2021: Cargamos los datos del proyecto
                Tb_Proyectos Proyecto = db.Tb_Proyectos.Where(y => y.Id == IdProyecto).FirstOrDefault();
                if (Proyecto == null)
                {
                    return Json(new { status = false, mensaje = "Error L61: Ocurrio un error enocntrar el proyecto con el ID " + IdProyecto.ToString() });
                }
                //HAY QUE CREAR UNA CLASE PARA COMBINAR LOS DATOS DEL PROYECTO JUNTO CON LA TABLA DE PAGOS Y ASI MOSTRAR TODO EN 1 SOLA LISTA
                #region Proyecto
                //Datos del proyecto
                DatosProyecto.Id = Proyecto.Id;
                DatosProyecto.Num_Proyecto_Cuenta = Proyecto.Num_Proyecto_Cuenta;
                DatosProyecto.FechaInicio = Proyecto.FechaInicio.ToShortDateString();
                DatosProyecto.MontoInicial = Proyecto.MontoInicial;
                DatosProyecto.MontoFinal = Proyecto.MontoFinal;
                DatosProyecto.Retorno = Proyecto.Retorno;
                DatosProyecto.Descripcion = Proyecto.Descripcion;
                DatosProyecto.Moneda = Proyecto.Moneda;
                #endregion

                #region Pagos
                //DATOS DE PAGO            
                //Buscamos todos los pagos que esten relacionados con el proyecto
                List<Tb_Pagos> PagosDB = db.Tb_Pagos.Where(w => w.Tb_Proyectos.Id == IdProyecto).ToList();
                if (PagosDB.Count > 0)
                {
                    
                    foreach (Tb_Pagos Pago in PagosDB)
                    {
                        cPagos o = new cPagos();
                        o.Id = Pago.Id;
                        o.FechaPago = Pago.FechaPago.ToShortDateString();
                        o.Monto = Pago.Monto;
                        o.Moneda = Pago.Moneda;
                        o.Referencia = Pago.Referencia;
                        o.TipoPago = Pago.TipoPago;
                        o.Retorno = Pago.Retorno;
                        o.RegistradoPor = Pago.RegistradoPor;
                        ListaPagos.Add(o);
                    }
                 

                }
                Subtotal = ListaPagos.Sum(x => x.Monto);
                TotalRetorno = ListaPagos.Sum(x => x.Retorno);
                MontoFinal = Subtotal - TotalRetorno;
                #endregion

            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            //Lista = Lista.OrderByDescending(x => x.FechaInicio).Reverse().ToList();
            return Json(new { status = true, DatosProyecto = DatosProyecto, ListaPagos = ListaPagos, Subtotal = Subtotal, TotalRetorno = TotalRetorno, MontoFinal= MontoFinal });
        }   
        public JsonResult RegistrarPago(string IdProyecto, string Monto, string REF, string TipoPago, string Retorno = "")
        {
            Tb_Pagos Pago = new Tb_Pagos();
            CultureInfo Culture = new CultureInfo("en-US");  //Definimos la cultura para que el separador de decimal sea por un Punto (.)    
            try
            {
                int ID_PROYECTO = Convert.ToInt32(IdProyecto);
                //Saul González 11/03/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(Monto.ToString()))
                {
                    return Json(new { status = false, mensaje = "Debe ingresar el monto pagado." });
                }
                if (Convert.ToDecimal(Monto, Culture) < 1)
                {
                    return Json(new { status = false, mensaje = "El monto debe ser mayor a 0." });
                }
                if (String.IsNullOrEmpty(REF))
                {
                    return Json(new { status = false, mensaje = "Debe ingresar una referencia." });
                }
             
                if (String.IsNullOrEmpty(TipoPago))
                {
                    return Json(new { status = false, mensaje = "Debe seleccionar un tipo de pago." });
                }
                //Saul Gonzalez 17/02/2021: Guardamos los datos del pago                    
                Pago.FechaPago = DateTime.Now;
                Pago.Monto = Convert.ToDecimal(Monto, Culture);
                //Pago.Moneda = Pago.Moneda;
                Pago.Referencia = REF;
                Pago.TipoPago = TipoPago;
                Pago.RegistradoPor = Session["Usuario"].ToString();
                //Validamos si se ingreso una cantidad en el campo de retorno, si esta vacia asignamos un 0
                if (String.IsNullOrEmpty(Retorno))
                {
                    Pago.Retorno = 0;
                }
                else
                {
                    Pago.Retorno = Convert.ToDecimal(Retorno, Culture);

                }            
        
                //Nos traemos los datos del proyecto
                Tb_Proyectos Proyecto = db.Tb_Proyectos.Where(y=> y.Id == ID_PROYECTO).FirstOrDefault();
                if (Proyecto == null)
                {
                    return Json(new { status = false, mensaje = "ERROR L-148: Ocurrió un error al encontrar el Id del proyecto." });
                }
                Pago.Tb_Proyectos = Proyecto;
                //Saul gonzalez 11/03/2021: Guardamos los datos
                db.Tb_Pagos.Add(Pago);
                db.SaveChanges();
            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Pago registrado correctamente." });
        }
    }
}