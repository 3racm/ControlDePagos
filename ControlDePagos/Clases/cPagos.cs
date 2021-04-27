using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Catalogo_3R_ACM.Clases
{
    public class cPagos
    {
        public int Id { get; set; }
        public string FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string Moneda { get; set; }
        public string Referencia { get; set; }
        public string TipoPago { get; set; }
        public Nullable<decimal> Retorno { get; set; }
        public string REF_Retorno { get; set; }
        public string RegistradoPor { get; set; }
        public string Notas { get; set; }
        public decimal ? Monto2 { get; set; }
        public string TipoPago2 { get; set; }
       

    }
}