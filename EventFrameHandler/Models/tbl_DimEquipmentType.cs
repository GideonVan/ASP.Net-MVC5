//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EventFrameHandler.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_DimEquipmentType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_DimEquipmentType()
        {
            this.tbl_DimSiteEquipment = new HashSet<tbl_DimSiteEquipment>();
            this.tbl_factMaintenanceAnnotations = new HashSet<tbl_factMaintenanceAnnotations>();
        }
    
        public string EquipmentType { get; set; }
        public int EquipmentId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_DimSiteEquipment> tbl_DimSiteEquipment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_factMaintenanceAnnotations> tbl_factMaintenanceAnnotations { get; set; }
    }
}