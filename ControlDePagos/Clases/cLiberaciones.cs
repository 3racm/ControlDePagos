using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Catalogo_3R_ACM.Clases
{
    public class cLiberaciones
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public string FechaRegistro { get; set; }
        public string Notas { get; set; }
        public string Num_Liberacion { get; set; }
        public string FolioRequisicion { get; set; }
    }
}