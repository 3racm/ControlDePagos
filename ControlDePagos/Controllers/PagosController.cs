﻿using Catalogo_3R_ACM.Clases;
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
            return View("Index");
        }

        public JsonResult CargarDatosProyecto(int IdProyecto = 0)
        {
            cProyectos DatosProyecto = new cProyectos();
            List<cPagos> ListaPagos = new List<cPagos>();
            //Variables que calculan el subtotal, totalRetorno y MontoFinal
            decimal ? Subtotal = 0;
            decimal ? SubtotalMonto2 = 0;
            decimal ? TotalRetorno = 0;
            decimal ? MontoFinal = 0;
            try
            {
                //Saul González 12/03/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(IdProyecto.ToString()))
                {
                    return Json(new { status = false, mensaje = "Error L52: Ocurrió un error al obtener el Id del proyecto." });
                }
                //Saul González 12/03/2021: Cargamos los datos del proyecto
                Tb_Proyectos Proyecto = db.Tb_Proyectos.Where(y => y.Id == IdProyecto).FirstOrDefault();
                if (Proyecto == null)
                {
                    return Json(new { status = false, mensaje = "Error L61: Ocurrió un error enocntrar el proyecto con el ID " + IdProyecto.ToString() });
                }
                //HAY QUE CREAR UNA CLASE PARA COMBINAR LOS DATOS DEL PROYECTO JUNTO CON LA TABLA DE PAGOS Y ASI MOSTRAR TODO EN 1 SOLA LISTA
                #region Proyecto
                //Datos del proyecto
                DatosProyecto.Id = Proyecto.Id;
                DatosProyecto.Num_Proyecto_Cuenta = Proyecto.Num_Proyecto_Cuenta;
                DatosProyecto.FechaInicio = Proyecto.FechaInicio.ToString("yyyy-MM-dd");
                DatosProyecto.MontoInicial = Proyecto.MontoInicial;
                DatosProyecto.MontoFinal = Proyecto.MontoFinal;
                DatosProyecto.Retorno = Proyecto.Retorno;
                DatosProyecto.Descripcion = Proyecto.Descripcion;
                DatosProyecto.Moneda = Proyecto.Moneda;
                DatosProyecto.Estado = Proyecto.Estado;
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
                        o.Referencia = Pago.Referencia;
                        o.TipoPago = Pago.TipoPago;
                        o.Retorno = Pago.Retorno;
                        o.REF_Retorno = Pago.REF_Retorno;
                        o.RegistradoPor = Pago.RegistradoPor;
                        o.Notas = Pago.Notas;
                        //Datos de combinaciones de pago
                        o.Monto2 = Pago.Monto2;
                        o.TipoPago2 = Pago.TipoPago2;
                        ListaPagos.Add(o);
                    }
                 

                }
                Subtotal = ListaPagos.Sum(x => x.Monto);
                SubtotalMonto2 = ListaPagos.Sum(y => y.Monto2);
                //sumamos los subtotales del monto1 con el monto2
                if (SubtotalMonto2 > 0)
                {
                    Subtotal = Subtotal + SubtotalMonto2;
                }                
                TotalRetorno = ListaPagos.Sum(x => x.Retorno);
                MontoFinal = Proyecto.MontoInicial - Subtotal + TotalRetorno;
                #endregion

            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            //Lista = Lista.OrderByDescending(x => x.FechaInicio).Reverse().ToList();
            return Json(new { status = true, DatosProyecto = DatosProyecto, ListaPagos = ListaPagos, Subtotal = Subtotal, TotalRetorno = TotalRetorno, MontoFinal= MontoFinal });
        }   
        public JsonResult RegistrarPago(string IdProyecto, string Monto, string REF, string TipoPago, string Retorno = "", string NotasPago = "", string Monto2 = "", string tipoPago2 ="", DateTime ? FechaESP = null)
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
                if (Monto2 != string.Empty && TipoPago == tipoPago2)
                {
                    return Json(new { status = false, mensaje = "No es posible tener una combinación con el mismo tipo de pago." });
                }
                //Saul Gonzalez 17/02/2021: Guardamos los datos del pago                    
                Pago.FechaPago = DateTime.Now;
                if (FechaESP != null)
                {
                    Pago.FechaPago = Convert.ToDateTime(FechaESP);
                }
                Pago.Monto = Convert.ToDecimal(Monto, Culture);
                //Saul Gonzalez 24/03/2021: Validamos si se agrego una segunda combinacion de pago
                if (!String.IsNullOrEmpty(Monto2))
                {
                    Pago.Monto2 = Convert.ToDecimal(Monto2, Culture);
                    Pago.TipoPago2 = tipoPago2;
                }               
                Pago.Referencia = REF;
                Pago.TipoPago = TipoPago;
                Pago.RegistradoPor = Session["Usuario"].ToString();
                Pago.Notas = NotasPago;
                //Validamos si se ingreso una cantidad en el campo de retorno, si esta vacia asignamos un 0
                if (String.IsNullOrEmpty(Retorno))
                {
                    Pago.Retorno = 0;
                    Pago.REF_Retorno = string.Empty;
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


        public JsonResult CargarPago(int IdPago)
        {
            cPagos Pago = new cPagos();
            CultureInfo Culture = new CultureInfo("en-US");  //Definimos la cultura para que el separador de decimal sea por un Punto (.)    
            try
            {   
                //Saul González 11/03/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(IdPago.ToString()))
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al obtener el Id del pago." });
                }
                //Saul Gonzalez 22/03/2021: Cargamos los datos del pago
                Tb_Pagos o = db.Tb_Pagos.Where(y => y.Id == IdPago).FirstOrDefault();
                if (o == null)
                {
                    return Json(new { status = false, mensaje = "ERROR L-196: Ocurrió un error al encontrar los datos del pago " + IdPago.ToString()});
                }
                Pago.Id = o.Id;
                Pago.Monto = o.Monto;
                Pago.TipoPago = o.TipoPago;
                Pago.Referencia = o.Referencia;
                Pago.Retorno = o.Retorno;                            
                Pago.REF_Retorno = o.REF_Retorno;                            
                Pago.Notas = o.Notas;
                Pago.FechaPago = o.FechaPago.ToString("yyyy-MM-dd");
                //Datos de pagos combinados
                Pago.Monto2 = o.Monto2;
                Pago.TipoPago2 = o.TipoPago2;

            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Pago registrado correctamente.", Datos = Pago });
        }

        public JsonResult ActualizarPago(string IdPago, string Monto, string REF, string TipoPago, string FechaPago, string Retorno = "", string REFRetorno ="", string NotasPago ="", string Monto2 ="", string tipoPago2 = "")
        {         
            CultureInfo Culture = new CultureInfo("en-US");  //Definimos la cultura para que el separador de decimal sea por un Punto (.)    
            try
            {
                int ID_PAGO = Convert.ToInt32(IdPago);
                if (Retorno == string.Empty)
                {
                    Retorno = "0";
                }
                //Saul González 23/03/2021: Validaciones de campos nulos
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
                if (String.IsNullOrEmpty(FechaPago))
                {
                    return Json(new { status = false, mensaje = "La fecha de pago no puede estar vacia." });
                }
                if (!String.IsNullOrEmpty(Retorno))
                {
                    if (!String.IsNullOrEmpty(Monto2))
                    {
                        var SumaMontos = Convert.ToDecimal(Monto, Culture) + Convert.ToDecimal(Monto2, Culture);
                        if (Convert.ToDecimal(Retorno, Culture) > Convert.ToDecimal(SumaMontos, Culture))
                        {
                            return Json(new { status = false, mensaje = "La cantidad de retorno no puede ser mayor a la suma total de los montos combinados." });
                        }
                    }
                    else
                    {
                        if (Convert.ToDecimal(Retorno, Culture) > Convert.ToDecimal(Monto, Culture))
                        {
                            return Json(new { status = false, mensaje = "La cantidad de retorno no puede ser mayor al monto." });
                        }
                    }
                   
                }               
                if (Convert.ToDecimal(Retorno,Culture) > 0 && String.IsNullOrEmpty(REFRetorno))
                {
                    return Json(new { status = false, mensaje = "Al ingresar una cantidad de retorno, debe también indicar la referencia." });
                }               
                if (!String.IsNullOrEmpty(REFRetorno) && Convert.ToDecimal(Retorno,Culture) < 1)
                {
                    return Json(new { status = false, mensaje = "Al ingresar una referencia de retorno, el monto de retorno debe ser mayor a 0." });
                }
                if (Monto2 != string.Empty && TipoPago == tipoPago2)
                {
                    return Json(new { status = false, mensaje = "No es posible tener una combinación con el mismo tipo de pago." });
                }
                //Nos traemos los datos del pago
                Tb_Pagos Pago = db.Tb_Pagos.Where(y => y.Id == ID_PAGO).FirstOrDefault();
                if (Pago == null)
                {
                    return Json(new { status = false, mensaje = "ERROR L-241: Ocurrio un error al obtener los datos del pago con el ID: " + IdPago });
                }
                Pago.Monto = Convert.ToDecimal(Monto, Culture);
                Pago.Referencia = REF;
                var HoraPago = DateTime.Now.TimeOfDay;
                Pago.FechaPago = Convert.ToDateTime(FechaPago).Add(HoraPago);
                Pago.TipoPago = TipoPago;                
                Pago.Notas = NotasPago;
                //Saul Gonzalez 24/03/2021: Validamos si se agrego una segunda combinacion de pago
                if (!String.IsNullOrEmpty(Monto2))
                {
                    Pago.Monto2 = Convert.ToDecimal(Monto2, Culture);
                    Pago.TipoPago2 = tipoPago2;
                }
                //Validamos si se ingreso una cantidad en el campo de retorno, si esta vacia asignamos un 0
                if (String.IsNullOrEmpty(Retorno))
                {
                    Pago.Retorno = 0;
                    Pago.REF_Retorno = string.Empty;
                }
                else
                {
                    Pago.Retorno = Convert.ToDecimal(Retorno, Culture);
                    Pago.REF_Retorno = REFRetorno;

                }
                //Saul gonzalez 23/03/2021: Guardamos los datos
                db.Tb_Pagos.Attach(Pago);
                db.Entry(Pago).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Datos actualizados" });
        }


        public JsonResult EliminarPago(int IdPago)
        {           
            try
            {
                //Saul González 11/03/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(IdPago.ToString()))
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al obtener el Id del pago." });
                }
                //Saul Gonzalez 22/03/2021: Cargamos los datos del pago
                Tb_Pagos Pago = db.Tb_Pagos.Where(y => y.Id == IdPago).FirstOrDefault();
                if (Pago == null)
                {
                    return Json(new { status = false, mensaje = "ERROR L-196: Ocurrió un error al encontrar los datos del pago " + IdPago.ToString() });
                }
                db.Tb_Pagos.Remove(Pago);
                db.SaveChanges();

            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Pago eliminado correctamente" });
        }
        public JsonResult Filtrar(int IdProyecto, string FiltroREF = "", string tipoPagoFiltro = "", DateTime ? FechaInicioFiltro = null, DateTime ? FechaFinalFiltro = null)
        {
            //cPagos Pago = new cPagos();
          
            List<cPagos> ListaPagos = new List<cPagos>();
            CultureInfo Culture = new CultureInfo("en-US");  //Definimos la cultura para que el separador de decimal sea por un Punto (.)    
            try
            {
                List<Tb_Pagos> Lista = new List<Tb_Pagos>();
                if (String.IsNullOrEmpty(IdProyecto.ToString()))
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al obtener el ID del proyecto." });
                }
                if (FechaInicioFiltro != null && FechaFinalFiltro == null)
                {
                    return Json(new { status = false, mensaje = "Al indicar una fecha de inicio debe establecer también una fecha final." });
                }
                if (FechaInicioFiltro == null && FechaFinalFiltro != null)
                {
                    return Json(new { status = false, mensaje = "Al indicar una fecha final debe establecer también una fecha inicial." });
                }
                //Primero filtramos por todos los pagos del proyecto
                Lista = db.Tb_Pagos.Where(x => x.Tb_Proyectos.Id == IdProyecto).ToList();

                //Saul Gonzalez 29/03/2021: Validamos en cascada que datos se mandaron para ir filtrando la lista
                if (FechaInicioFiltro != null && FechaFinalFiltro != null)
                {
                    Lista = Lista.Where(y => y.FechaPago >= FechaInicioFiltro && y.FechaPago <= FechaFinalFiltro).ToList();
                }                                          
                if (tipoPagoFiltro != string.Empty)
                {
                    Lista = Lista.Where(y => y.TipoPago == tipoPagoFiltro || y.TipoPago2 == tipoPagoFiltro).ToList();
                }
                if (FiltroREF != string.Empty)
                {
                    Lista = Lista.Where(y => y.Referencia == FiltroREF).ToList();
                }
                if (Lista.Count < 1)
                {
                    return Json(new { status = false, mensaje = "No se encontraron registros de pagos con los datos que indico." });
                }
                foreach (Tb_Pagos Pago in Lista)
                {
                    cPagos o = new cPagos();
                    o.Id = Pago.Id;
                    o.FechaPago = Pago.FechaPago.ToShortDateString();
                    o.Monto = Pago.Monto;
                    o.Referencia = Pago.Referencia;
                    o.TipoPago = Pago.TipoPago;
                    o.Retorno = Pago.Retorno;
                    o.RegistradoPor = Pago.RegistradoPor;
                    o.Notas = Pago.Notas;                  
                    //Datos de combinaciones de pago
                    o.Monto2 = Pago.Monto2;
                    o.TipoPago2 = Pago.TipoPago2;
                    ListaPagos.Add(o);             
                }

            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Filtros aplicados", Lista = ListaPagos });
        }
    }
}