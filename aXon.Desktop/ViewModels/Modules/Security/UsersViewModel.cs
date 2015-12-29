using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aXon.Data;

namespace aXon.Desktop.ViewModels.Modules.Security
{
    public class UsersViewModel: BaseViewModel
    {
        private ObservableCollection<User> _mainData;
        private User _selectedRow;
        private bool _editMode;

        public System.Collections.ObjectModel.ObservableCollection<User> MainData
        {
            get { return _mainData; }
            set
            {
                if (Equals(value, _mainData)) return;
                _mainData = value;
                OnPropertyChanged();
            }
        }

        public User SelectedRow
        {
            get { return _selectedRow; }
            set
            {
                if (Equals(value, _selectedRow)) return;
                _selectedRow = value;
                OnPropertyChanged();
            }
        }

        public bool EditMode
        {
            get { return _editMode; }
            set
            {
                if (value == _editMode) return;
                _editMode = value;
                OnPropertyChanged();
            }
        }
    }
}

