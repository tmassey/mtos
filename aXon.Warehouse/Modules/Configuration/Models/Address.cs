using System;

namespace aXon.Warehouse.Modules.Configuration.Models
{
    public class Address : BaseModel
    {
        private string _postalCode;
        private Guid _cityId;
        private Guid _stateId;
        private Guid _countryId;
        private string _line3;
        private string _line2;
        private string _line1;

        public string Line1
        {
            get { return _line1; }
            set
            {
                if (value == _line1) return;
                _line1 = value;
                OnPropertyChanged();
            }
        }

        public string Line2
        {
            get { return _line2; }
            set
            {
                if (value == _line2) return;
                _line2 = value;
                OnPropertyChanged();
            }
        }

        public string Line3
        {
            get { return _line3; }
            set
            {
                if (value == _line3) return;
                _line3 = value;
                OnPropertyChanged();
            }
        }

        public Guid CountryId
        {
            get { return _countryId; }
            set
            {
                if (value.Equals(_countryId)) return;
                _countryId = value;
                OnPropertyChanged();
            }
        }

        public Guid StateId
        {
            get { return _stateId; }
            set
            {
                if (value.Equals(_stateId)) return;
                _stateId = value;
                OnPropertyChanged();
            }
        }

        public Guid CityId
        {
            get { return _cityId; }
            set
            {
                if (value.Equals(_cityId)) return;
                _cityId = value;
                OnPropertyChanged();
            }
        }

        public string PostalCode
        {
            get { return _postalCode; }
            set
            {
                if (value == _postalCode) return;
                _postalCode = value;
                OnPropertyChanged();
            }
        }
    }
}