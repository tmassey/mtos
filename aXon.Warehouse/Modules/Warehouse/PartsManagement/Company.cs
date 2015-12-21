using aXon.Warehouse.Modules.Configuration.Models;

namespace aXon.Warehouse.Modules.Warehouse.PartsManagement
{
    public class Company : BaseModel
    {
        public string Name { get; set; }
        public Address HeadquartersAddress { get; set; }

    }
}