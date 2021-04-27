using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Catalogo_3R_ACM.Clases
{
    public class cUsuario
    {
        public int Id_usuario { get; set; }
        public string Nombre { get; set; }
        public string CorreoElectronico { get; set; }
        public string Contraseña { get; set; }
        public string Permiso { get; set; }
        public System.DateTime FechaRegistro { get; set; }
        public bool status { get; set; }
        public string mensaje { get; set; }
        public string FechaTexto { get; set; }
    }
}