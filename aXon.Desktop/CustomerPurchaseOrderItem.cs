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
    
    public partial class CustomerPurchaseOrderItem
    {
        public System.Guid Id { get; set; }
        public System.DateTimeOffset CreateDateTime { get; set; }
        public System.DateTimeOffset LastEditDateTime { get; set; }
        public System.Guid ModifiedBy { get; set; }
        public System.Guid CreatedBy { get; set; }
        public bool IsActiveRecord { get; set; }
        public System.Guid CompanyId { get; set; }
        public System.Guid WarehouseId { get; set; }
        public System.Guid PartId { get; set; }
        public int Qty { get; set; }
        public System.Guid PurchaseOrderId { get; set; }
        public System.Guid CustomerId { get; set; }
    
        public virtual Company Company { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual CustomerPurchaseOrder CustomerPurchaseOrder { get; set; }
        public virtual Part Part { get; set; }
        public virtual WareHouse WareHouse { get; set; }
    }
}
