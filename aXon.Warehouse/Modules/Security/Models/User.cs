namespace aXon.Warehouse.Modules.Security.Models
{
    public class User : BaseModel
    {
        private string _password;
        private string _userName;
        private string _emailAddress;

        public string Password
        {
            get { return _password; }
            set
            {
                if (value == _password) return;
                _password = value;
                OnPropertyChanged();
            }
        }
        public string EmailAddress
        {
            get { return _emailAddress; }
            set
            {
                if (value == _emailAddress) return;
                _emailAddress = value;
                OnPropertyChanged();
            }
        }
        public string UserName
        {
            get { return _userName; }
            set
            {
                if (value == _userName) return;
                _userName = value;
                OnPropertyChanged();
            }
        }
    }
}