namespace aXon.Warehouse.Modules.Warehouse.PartsManagement
{
    public class Carrier : Company
    {
        private string _scacCode;

        public string ScacCode
        {
            get { return _scacCode; }
            set
            {
                if (value == _scacCode) return;
                _scacCode = value;
                OnPropertyChanged();
            }
        }
    }
}