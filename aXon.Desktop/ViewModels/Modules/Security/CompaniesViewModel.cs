using System.Collections.ObjectModel;

namespace aXon.Desktop.ViewModels.Modules.Security
{
    public class CompaniesViewModel : BaseViewModel
    {
        private ObservableCollection<Company> _mainData;
        private Company _selectedRow;
        private bool _editMode;
        private Address _selectedRowAddress;

        public ObservableCollection<Company> MainData
        {
            get { return _mainData; }
            set
            {
                if (Equals(value, _mainData)) return;
                _mainData = value;
                OnPropertyChanged();
            }
        }

        public Address SelectedRowAddress
        {
            get { return _selectedRowAddress; }
            set
            {
                if (Equals(value, _selectedRowAddress)) return;
                _selectedRowAddress = value;
                
                OnPropertyChanged();
            }
        }

        public Company SelectedRow
        {
            get { return _selectedRow; }
            set
            {
                if (Equals(value, _selectedRow)) return;
                _selectedRow = value;
                if (value.Address != null)
                    SelectedRowAddress = value.Address;
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