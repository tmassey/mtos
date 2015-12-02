using System.Collections.ObjectModel;

namespace aXon.Warehouse.Modules.Configuration.Models
{
    public class Country : BaseModel
    {
        private ObservableCollection<State> _states;
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

        public ObservableCollection<State> States
        {
            get { return _states; }
            set
            {
                if (Equals(value, _states)) return;
                _states = value;
                OnPropertyChanged();
            }
        }
    }
}