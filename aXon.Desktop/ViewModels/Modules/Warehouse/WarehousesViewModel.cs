using System.Collections.ObjectModel;
using aXon.Data;

namespace aXon.Desktop.ViewModels.Modules.Warehouse
{
    public class WarehousesViewModel : BaseViewModel
    {
        private ObservableCollection<WareHouse> _mainData;
        private WareHouse _selectedRow;
        private bool _editMode;
        private ObservableCollection<Company> _companies;

        public System.Collections.ObjectModel.ObservableCollection<WareHouse> MainData
        {
            get { return _mainData; }
            set
            {
                if (Equals(value, _mainData)) return;
                _mainData = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Company> Companies
        {
            get { return _companies; }
            set
            {
                if (Equals(value, _companies)) return;
                _companies = value;
                OnPropertyChanged();
            }
        }

        public WareHouse SelectedRow
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