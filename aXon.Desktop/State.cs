//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace aXon.Desktop
{
    using System;
    using System.Collections.Generic;
    
    public partial class State
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public State()
        {
            this.Cities = new HashSet<City>();
        }
    
        public System.Guid Id { get; set; }
        public System.DateTimeOffset CreateDateTime { get; set; }
        public System.DateTimeOffset LastEditDateTime { get; set; }
        public System.Guid ModifiedBy { get; set; }
        public System.Guid CreatedBy { get; set; }
        public bool IsActiveRecord { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Abreviation { get; set; }
        public System.Guid CountryId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<City> Cities { get; set; }
        public virtual Country Country { get; set; }
    }
}
