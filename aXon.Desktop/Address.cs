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
    
    public partial class Address
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Address()
        {
            this.Carriers = new HashSet<Carrier>();
            this.Customers = new HashSet<Customer>();
            this.Companies = new HashSet<Company>();
            this.Vendors = new HashSet<Vendor>();
        }
    
        public System.Guid Id { get; set; }
        public System.DateTimeOffset CreateDateTime { get; set; }
        public System.DateTimeOffset LastEditDateTime { get; set; }
        public System.Guid ModifiedBy { get; set; }
        public System.Guid CreatedBy { get; set; }
        public bool IsActiveRecord { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public Nullable<System.Guid> CountryId { get; set; }
        public Nullable<System.Guid> StateId { get; set; }
        public Nullable<System.Guid> CityId { get; set; }
        public string PostalCode { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Carrier> Carriers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Customer> Customers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Company> Companies { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Vendor> Vendors { get; set; }
    }
}
