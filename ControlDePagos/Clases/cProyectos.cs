using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Catalogo_3R_ACM.Clases
{
    public class cProyectos
    {
        public int Id { get; set; }
        public string Num_Proyecto_Cuenta { get; set; }
        public string FechaInicio { get; set; }
        public decimal MontoInicial { get; set; }
        public string Moneda { get; set; }
        public Nullable<decimal> MontoFinal { get; set; }
        public Nullable<decimal> Retorno { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
    }
}