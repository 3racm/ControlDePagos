using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Catalogo_3R_ACM.Clases
{
    public class cRequisicion
    {
        public int Id { get; set; }
        public string Folio { get; set; }
        public string Cuenta_Cargo { get; set; }
        public decimal Total { get; set; }
        public string Moneda { get; set; }
        public string Descripcion { get; set; }
        public string Solicitud { get; set; }
        public string TipoReq { get; set; }
        public System.DateTime FechaRegistro { get; set; }
    }
}