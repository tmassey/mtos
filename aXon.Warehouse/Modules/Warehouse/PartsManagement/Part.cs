using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aXon.Warehouse.Modules.Warehouse.PartsManagement
{
    public class Part:CompanyBaseModel
    {
        public Guid ParentPartId { get; set; }
        public string PartNumber { get; set; }
        public string Revision { get; set; }
        public string Description { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public Guid DimentionsUnitOfMeasureId { get; set; }
        public decimal WeightinPounds { get; set; }
        

    }

    public class PartImage : CompanyBaseModel
    {
        public string FileName { get; set; }
        public Guid StorageId { get; set; }
        public Guid PartId { get; set; }
    }

    public class PartFile : CompanyBaseModel
    {
        public string FileName { get; set; }
        public Guid StorageId { get; set; }
        public Guid PartId { get; set; }
    }

    public class PartVendors:CompanyBaseModel
    {
        
    }
}
