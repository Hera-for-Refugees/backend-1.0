//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Hera.Data.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class SType_Media
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SType_Media()
        {
            this.Category_Media = new HashSet<Category_Media>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int MinWidth { get; set; }
        public int MinHeight { get; set; }
        public bool IsVideo { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Category_Media> Category_Media { get; set; }
    }
}