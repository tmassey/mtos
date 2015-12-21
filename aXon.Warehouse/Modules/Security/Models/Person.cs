using System;

namespace aXon.Warehouse.Modules.Security.Models
{
    public class Person:User
    {
        
        
        private string _lastName;
        private string _middleName;
        private string _firstName;
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

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (value == _firstName) return;
                _firstName = value;
                OnPropertyChanged();
            }
        }

        public string MiddleName
        {
            get { return _middleName; }
            set
            {
                if (value == _middleName) return;
                _middleName = value;
                OnPropertyChanged();
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (value == _lastName) return;
                _lastName = value;
                OnPropertyChanged();
            }
        }

       

        
    }
}
