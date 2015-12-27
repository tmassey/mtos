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
    
    public partial class WarehousePosition
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WarehousePosition()
        {
            this.WarehouseNeuralNetworks = new HashSet<WarehouseNeuralNetwork>();
            this.WarehouseNeuralNetworks1 = new HashSet<WarehouseNeuralNetwork>();
            this.WarehouseRobots = new HashSet<WarehouseRobot>();
        }
    
        public System.Guid Id { get; set; }
        public System.DateTimeOffset CreateDateTime { get; set; }
        public System.DateTimeOffset LastEditDateTime { get; set; }
        public System.Guid ModifiedBy { get; set; }
        public System.Guid CreatedBy { get; set; }
        public bool IsActiveRecord { get; set; }
        public System.Guid CompanyId { get; set; }
        public System.Guid WarehouseId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int MapMode { get; set; }
        public int StorageType { get; set; }
    
        public virtual Company Company { get; set; }
        public virtual WareHouse WareHouse { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WarehouseNeuralNetwork> WarehouseNeuralNetworks { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WarehouseNeuralNetwork> WarehouseNeuralNetworks1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WarehouseRobot> WarehouseRobots { get; set; }
    }
}