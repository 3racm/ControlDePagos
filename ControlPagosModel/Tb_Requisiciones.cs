//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ControlPagosModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tb_Requisiciones
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tb_Requisiciones()
        {
            this.Tb_Liberaciones = new HashSet<Tb_Liberaciones>();
        }
    
        public int Id { get; set; }
        public string Folio { get; set; }
        public string Cuenta_Cargo { get; set; }
        public decimal Total { get; set; }
        public string Moneda { get; set; }
        public string Descripcion { get; set; }
        public string Solicitud { get; set; }
        public string TipoReq { get; set; }
        public System.DateTime FechaRegistro { get; set; }
        public string NumeroDeFactura { get; set; }
        public Nullable<decimal> TotalCargoCliente { get; set; }
        public string FacturaRegistradaPor { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tb_Liberaciones> Tb_Liberaciones { get; set; }
    }
}
