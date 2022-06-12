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
    public class ProyectosController : Controller
    {
        ControlPagosModel.ControlDeGastosEntities db = new ControlPagosModel.ControlDeGastosEntities();
        // GET: Proyectos
        public ActionResult Index()
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
                    return RedirectToAction("Index", "Requisiciones");
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
            List<cProyectos> Lista = new List<cProyectos>();
            try
            {
                //Saul Gonzalez 14/10/2020: Obtenemos la lista de proveedores
                List<Tb_Proyectos> tablaProyectos = db.Tb_Proyectos.ToList();
                foreach (Tb_Proyectos Proyecto in tablaProyectos)
                {
                    cProyectos o = new cProyectos();
                    o.Id = Proyecto.Id;
                    o.Num_Proyecto_Cuenta = Proyecto.Num_Proyecto_Cuenta;
                    o.FechaInicio = Proyecto.FechaInicio.ToShortDateString();                   
                    o.MontoInicial = Proyecto.MontoInicial;
                    o.Moneda = Proyecto.Moneda;
                    o.MontoFinal = Proyecto.MontoFinal;
                    o.Retorno = Proyecto.Retorno;
                    o.Descripcion = Proyecto.Descripcion;
                    o.Estado = Proyecto.Estado;
                    Lista.Add(o);
                }
                Lista = Lista.OrderByDescending(o=> Convert.ToDateTime(o.FechaInicio)).ToList();
            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            //Lista = Lista.OrderByDescending(x => x.FechaInicio).Reverse().ToList();
            return Json(Lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RegistrarProyecto(string NoProyecto, string MontoInicial, string Moneda, string Descripcion, string FechaEspecifica = "")
        {
            Tb_Proyectos Proyecto = new Tb_Proyectos();
            CultureInfo Culture = new CultureInfo("en-US");  //Definimos la cultura para que el separador de decimal sea por un Punto (.)    
            try
            {
                //Saul González 11/03/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(NoProyecto))
                {
                    return Json(new { status = false, mensaje = "Debe indicar el número de proyecto o cuenta." });
                }
                if (String.IsNullOrEmpty(MontoInicial))
                {
                    return Json(new { status = false, mensaje = "El monto inicial no puede estar vacío." });
                }
                if (Convert.ToDecimal(MontoInicial, Culture) < 1)
                {
                    return Json(new { status = false, mensaje = "El monto inicial debe ser mayor a 0." });
                }
                if (String.IsNullOrEmpty(Moneda))
                {
                    return Json(new { status = false, mensaje = "Debe establecer una moneda para el proyecto." });
                }
              
                if (String.IsNullOrEmpty(Descripcion))
                {
                    return Json(new { status = false, mensaje = "El campo descripción es obligatorio." });
                }
                //Saul Gonzalez 11/03/2021: Consultamos si el #Proyecto que se esta intentando registrar ya existe.               
                Tb_Proyectos RegistroExistente = db.Tb_Proyectos.Where(y => y.Num_Proyecto_Cuenta.Equals(NoProyecto)).FirstOrDefault();
                //Saul Gonzalez 01/05/2022: Se quita esta parte de la validacion ya que se requirio una modificacion para que se puedan registar codigos iguales pero fechas distintas ya que ellos manejan los proyectos por año
                //if (ValidarProyecto != null)
                //{
                //    return Json(new { status = false, mensaje = "El número de proyecto o cuenta que ingreso ya existe." });
                //}
                //Saul gonzalez 11/03/2021: Asignamos valores al objeto              
                Proyecto.Num_Proyecto_Cuenta = NoProyecto;
                if (FechaEspecifica == string.Empty)
                {
                    Proyecto.FechaInicio = DateTime.Now;
                }
                else
                {
                    Proyecto.FechaInicio = Convert.ToDateTime(FechaEspecifica);
                }
                //Saul Gonzalez 01/05/2022: Si existe un proyecto con el mismo codigo solo validamos que el año no sea el mismo que se esta registrando
                int? FechaVieja = null;
                if (RegistroExistente != null)
                {
                    FechaVieja = RegistroExistente.FechaInicio.Year;
                }
                
                int FechaNueva = Proyecto.FechaInicio.Year;
                if (FechaNueva == FechaVieja)
                {
                    return Json(new { status = false, mensaje = "El código del proyecto que estas intentando registrar ya existe para el año " + FechaVieja.ToString() +"." });
                }                
                Proyecto.MontoInicial = Convert.ToDecimal(MontoInicial, Culture);               
                Proyecto.Moneda = Moneda;
                Proyecto.MontoFinal = 0;
                Proyecto.Retorno = 0;
                Proyecto.Descripcion = Descripcion;
                Proyecto.Estado = "Activo";
                //Saul gonzalez 11/03/2021: Guardamos los datos
                db.Tb_Proyectos.Add(Proyecto);
                db.SaveChanges();
            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Datos guardados", Id = Proyecto.Id });
        }

        public JsonResult ActualizarMontoInicial(string IdProyectoP,string MontoInicial)
        {
            CultureInfo CultureEN = new CultureInfo("en-US");  //Definimos la cultura para que el separador de decimal sea por un Punto (.)    
            CultureInfo CultureES = new CultureInfo("es-ES");
            try
            {

                //Saul González 23/03/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(IdProyectoP))
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al obtener el Id del proyecto, no fue posible actualizar el monto." });
                }
                if (String.IsNullOrEmpty(MontoInicial))
                {
                    return Json(new { status = false, mensaje = "El monto inicial no puede estar vacío." });
                }
                int Id_Proyecto = Int32.Parse(IdProyectoP);
                //Saul Gonzalez 23/03/2021: Consultamos los datos del proyecto
                Tb_Proyectos Proyecto = db.Tb_Proyectos.Where(y => y.Id == Id_Proyecto).FirstOrDefault();
                if (Proyecto == null)
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al buscar el proyecto con el ID: " + IdProyectoP });
                }
                //Saul gonzalez 23/03/2021: Actualizamos el monto inicial del proyecto
                if (MontoInicial.Contains("."))
                {
                    Proyecto.MontoInicial = Convert.ToDecimal(MontoInicial, CultureEN);
                }
                else if (MontoInicial.Contains(","))
                {
                    Proyecto.MontoInicial = Convert.ToDecimal(MontoInicial, CultureES);
                }
               // Proyecto.MontoInicial = Convert.ToDecimal(MontoInicial);              

                //Saul gonzalez 23/03/2021: Guardamos los datos
                db.Tb_Proyectos.Attach(Proyecto);
                db.Entry(Proyecto).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Datos guardados"});
        }
        public JsonResult ActualizarFechaInicial(string IdProyectoP, string FechaInicio)
        {
            CultureInfo Culture = new CultureInfo("en-US");  //Definimos la cultura para que el separador de decimal sea por un Punto (.)    
            try
            {

                //Saul González 13/04/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(IdProyectoP))
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al obtener el Id del proyecto, no fue posible actualizar la fecha." });
                }
                if (String.IsNullOrEmpty(FechaInicio))
                {
                    return Json(new { status = false, mensaje = "La fecha inicial no puede estar vacía." });
                }
                int Id_Proyecto = Int32.Parse(IdProyectoP);
                //Saul Gonzalez 13/04/2021: Consultamos los datos del proyecto
                Tb_Proyectos Proyecto = db.Tb_Proyectos.Where(y => y.Id == Id_Proyecto).FirstOrDefault();
                if (Proyecto == null)
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al buscar el proyecto con el ID: " + IdProyectoP });
                }
                //Saul gonzalez 13/04/2021: Actualizamos el monto inicial del proyecto
                Proyecto.FechaInicio = Convert.ToDateTime(FechaInicio);

                //Saul gonzalez 13/04/2021: Guardamos los datos
                db.Tb_Proyectos.Attach(Proyecto);
                db.Entry(Proyecto).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Datos guardados" });
        }
        public JsonResult ActualizarDescripcion(string IdProyectoP, string Descripcion)
        {
            CultureInfo Culture = new CultureInfo("en-US");  //Definimos la cultura para que el separador de decimal sea por un Punto (.)    
            try
            {

                //Saul González 14/04/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(IdProyectoP))
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al obtener el Id del proyecto, no fue posible actualizar la Descripcion." });
                }
                if (String.IsNullOrEmpty(Descripcion))
                {
                    return Json(new { status = false, mensaje = "El campo descripcion no puede estar vacio." });
                }
                int Id_Proyecto = Int32.Parse(IdProyectoP);
                //Saul Gonzalez 13/04/2021: Consultamos los datos del proyecto
                Tb_Proyectos Proyecto = db.Tb_Proyectos.Where(y => y.Id == Id_Proyecto).FirstOrDefault();
                if (Proyecto == null)
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al buscar el proyecto con el ID: " + IdProyectoP });
                }
                //Saul gonzalez 14/04/2021: Actualizamos el monto inicial del proyecto
                Proyecto.Descripcion = Descripcion;
                //Saul gonzalez 14/04/2021: Guardamos los datos
                db.Tb_Proyectos.Attach(Proyecto);
                db.Entry(Proyecto).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Descripcion actualizada" });
        }
        


        public JsonResult EliminarPago(int Id)
        {
            try
            {
                //Saul González 11/03/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(Id.ToString()))
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al obtener el Id del proyecto." });
                }
                //Saul Gonzalez 22/03/2021: Cargamos los datos del pago
                Tb_Proyectos Proyecto = db.Tb_Proyectos.Where(y => y.Id == Id).FirstOrDefault();
                if (Proyecto == null)
                {
                    return Json(new { status = false, mensaje = "ERROR L-178: Ocurrió un error al encontrar los datos del proyecto " + Id.ToString() });
                }
                if (Proyecto.Tb_Pagos.Count > 0)
                {
                    db.Tb_Pagos.RemoveRange(Proyecto.Tb_Pagos);
                }
                db.Tb_Proyectos.Remove(Proyecto);
                db.SaveChanges();

            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Proyecto eliminado correctamente" });
        }

        public JsonResult CerrarProyecto(int IdProyecto)
        {
            try
            {
                //Saul González 11/03/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(IdProyecto.ToString()))
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al obtener el Id del proyecto." });
                }
                //Saul Gonzalez 22/03/2021: Cargamos los datos del pago
                Tb_Proyectos Proyecto = db.Tb_Proyectos.Where(y => y.Id == IdProyecto).FirstOrDefault();
                if (Proyecto == null)
                {
                    return Json(new { status = false, mensaje = "ERROR L-178: Ocurrió un error al encontrar los datos del proyecto " + IdProyecto.ToString() });
                }
                Proyecto.Estado = "Cerrado";

                db.Tb_Proyectos.Attach(Proyecto);
                db.Entry(Proyecto).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Proyecto eliminado correctamente" });
        }

        public JsonResult ReabrirProyecto(int IdProyecto)
        {
            try
            {
                //Saul González 11/03/2021: Validaciones de campos nulos
                if (String.IsNullOrEmpty(IdProyecto.ToString()))
                {
                    return Json(new { status = false, mensaje = "Ocurrió un error al obtener el Id del proyecto." });
                }
                //Saul Gonzalez 22/03/2021: Cargamos los datos del pago
                Tb_Proyectos Proyecto = db.Tb_Proyectos.Where(y => y.Id == IdProyecto).FirstOrDefault();
                if (Proyecto == null)
                {
                    return Json(new { status = false, mensaje = "ERROR L-178: Ocurrió un error al encontrar los datos del proyecto " + IdProyecto.ToString() });
                }
                Proyecto.Estado = "Reabierto";

                db.Tb_Proyectos.Attach(Proyecto);
                db.Entry(Proyecto).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(new { status = true, mensaje = "Proyecto eliminado correctamente" });
        }
    }
}