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
    
    public partial class Tb_Proyectos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tb_Proyectos()
        {
            this.Tb_Pagos = new HashSet<Tb_Pagos>();
        }
    
        public int Id { get; set; }
        public string Num_Proyecto_Cuenta { get; set; }
        public System.DateTime FechaInicio { get; set; }
        public decimal MontoInicial { get; set; }
        public string Moneda { get; set; }
        public Nullable<decimal> MontoFinal { get; set; }
        public Nullable<decimal> Retorno { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Tb_Pagos> Tb_Pagos { get; set; }
    }
}
