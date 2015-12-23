using System;
using System.Windows.Controls;

namespace aXon.Desktop.Pages.Modules.Security
{
    /// <summary>
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class Users : AxonScreen
    {
        public Users()
        {
            InitializeComponent();
            ScreenName = "Users";
            ModuleName = "Security";
            SourcePath = new Uri("/Pages/Modules/Security/Users.xaml", UriKind.Relative);
        }
    }
}
