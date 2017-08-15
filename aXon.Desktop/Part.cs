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
    
    public partial class Part
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Part()
        {
            this.BasePurchaseOrderItems = new HashSet<BasePurchaseOrderItem>();
            this.CustomerPurchaseOrderItems = new HashSet<CustomerPurchaseOrderItem>();
            this.PartVendors = new HashSet<PartVendor>();
            this.ReceiptItems = new HashSet<ReceiptItem>();
            this.VendorPurchaseOrderItems = new HashSet<VendorPurchaseOrderItem>();
        }
    
        public System.Guid Id { get; set; }
        public System.DateTimeOffset CreateDateTime { get; set; }
        public System.DateTimeOffset LastEditDateTime { get; set; }
        public System.Guid ModifiedBy { get; set; }
        public System.Guid CreatedBy { get; set; }
        public bool IsActiveRecord { get; set; }
        public System.Guid CompanyId { get; set; }
        public string PartNumber { get; set; }
        public string Revision { get; set; }
        public Nullable<double> Length { get; set; }
        public Nullable<double> Width { get; set; }
        public Nullable<double> Height { get; set; }
        public Nullable<double> Weight { get; set; }
        public string Description { get; set; }
        public System.Guid WarehouseId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BasePurchaseOrderItem> BasePurchaseOrderItems { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CustomerPurchaseOrderItem> CustomerPurchaseOrderItems { get; set; }
        public virtual WareHouse WareHouse { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PartVendor> PartVendors { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReceiptItem> ReceiptItems { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VendorPurchaseOrderItem> VendorPurchaseOrderItems { get; set; }
    }
}