using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace aXon.Desktop
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : ModernDialog
    {
        public Login()
        {
            InitializeComponent();

            // define the dialog buttons
            this.Buttons = new Button[] { this.OkButton, this.CancelButton };
            
            OkButton.Click += OkButton_Click;
            CancelButton.Click += CancelButton_Click;
            Closing += Login_Closing;
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
                _authenticated = true;
            else
            {
                _authenticated = false;
                Error.Visibility=Visibility.Visible;
            }


        }
    }
}
