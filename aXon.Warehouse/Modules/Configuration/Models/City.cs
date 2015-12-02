using System.Collections.ObjectModel;

namespace aXon.Warehouse.Modules.Configuration.Models
{
    public class City : BaseModel
    {
        private ObservableCollection<string> _postalCodes;
        private string _abreviation;
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Abreviation
        {
            get { return _abreviation; }
            set
            {
                if (value == _abreviation) return;
                _abreviation = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> PostalCodes
        {
            get { return _postalCodes; }
            set
            {
                if (Equals(value, _postalCodes)) return;
                _postalCodes = value;
                OnPropertyChanged();
            }
        }
    }
}