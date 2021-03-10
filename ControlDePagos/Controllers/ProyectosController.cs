using Catalogo_3R_ACM.Clases;
using ControlPagosModel;
using System;
using System.Collections.Generic;
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
                    Lista.Add(o);
                }
            }
            catch (Exception error)
            {
                return Json(new { status = false, mensaje = error.Message });
            }
            return Json(Lista, JsonRequestBehavior.AllowGet);
        }
    }
}