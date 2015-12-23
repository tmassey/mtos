using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aXon.Desktop.ViewModels.Modules.Security
{
    public class UsersViewModel: BaseViewModel
    {
        public System.Collections.ObjectModel.ObservableCollection<User> MainData { get; set; }
        public User SelectedRow { get; set; }
    }
}
