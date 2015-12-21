using System;

namespace aXon.Warehouse
{
    public class CompanyBaseModel : BaseModel
    {
        private Guid _companyId;

        public Guid CompanyId
        {
            get { return _companyId; }
            set
            {
                if (value.Equals(_companyId)) return;
                _companyId = value;
                OnPropertyChanged();
            }
        }
    }
}