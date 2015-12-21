using System;
using aXon.Warehouse.Modules.Configuration.Models;

namespace aXon.Warehouse.Modules.Warehouse.PartsManagement
{
    public class Warehouse : BaseModel
    {
        public Guid CompanyId { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }

    }
}