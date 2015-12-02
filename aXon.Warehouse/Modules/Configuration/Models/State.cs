using System.Collections.ObjectModel;

namespace aXon.Warehouse.Modules.Configuration.Models
{
    public class State : BaseModel
    {
        private ObservableCollection<City> _cities;
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

        public ObservableCollection<City> Cities
        {
            get { return _cities; }
            set
            {
                if (Equals(value, _cities)) return;
                _cities = value;
                OnPropertyChanged();
            }
        }
    }
}