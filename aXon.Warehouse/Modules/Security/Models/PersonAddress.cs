using System;
using aXon.Warehouse.Modules.Configuration.Models;

namespace aXon.Warehouse.Modules.Security.Models
{
    public class PersonAddress : Address
    {
        private bool _isHomeAddress;
        private Guid _personId;

        public Guid PersonId
        {
            get { return _personId; }
            set
            {
                if (value.Equals(_personId)) return;
                _personId = value;
                OnPropertyChanged();
            }
        }

        public bool IsHomeAddress
        {
            get { return _isHomeAddress; }
            set
            {
                if (value == _isHomeAddress) return;
                _isHomeAddress = value;
                OnPropertyChanged();
            }
        }
    }
}