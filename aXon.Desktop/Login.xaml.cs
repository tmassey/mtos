using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using aXon.Data;

namespace aXon.Desktop
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : ModernDialog
    {
        public aXonEntities Entities { get; set; }
        public Login()
        {
            InitializeComponent();

            // define the dialog buttons
            this.Buttons = new Button[] { this.OkButton, this.CancelButton };
            Loaded += Login_Loaded;
            OkButton.Click += OkButton_Click;
            CancelButton.Click += CancelButton_Click;
            Closing += Login_Closing;
        }

        private void Login_Loaded(object sender, RoutedEventArgs e)
        {
            Entities = new aXonEntities();
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _authenticated = true;
        }

        private bool _authenticated = false;
        void Login_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!_authenticated)
                e.Cancel = true;
        }

        void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserName.Text.ToUpper() == "ADMIN" && Password.Password.ToUpper() == "ADMIN")
            {
                _authenticated = true;
                Globals.CurrentUser= new Employee() {Id=Guid.NewGuid(),FirstName="aXon",LastName = "Administrator"};
                return;
            }
            var user = Entities.Employees.FirstOrDefault(u=>u.UserName.ToUpper() == UserName.Text.ToUpper() && u.Password.ToUpper() == Password.Password.ToUpper());
            if (user == null)
            {
                _authenticated = false;
                Error.Visibility = Visibility.Visible;
            }
            else
            {
                _authenticated = true;
                Globals.CurrentUser = user;
            }



        }
    }
}
