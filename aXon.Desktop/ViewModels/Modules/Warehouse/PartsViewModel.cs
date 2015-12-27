using System.Collections.ObjectModel;

namespace aXon.Desktop.ViewModels.Modules.Warehouse
{
    public class PartsViewModel : BaseViewModel
    {
        private ObservableCollection<Part> _mainData;
        private Part _selectedRow;
        private bool _editMode;
        private ObservableCollection<Company> _companies;
        private ObservableCollection<Vendor> _vendors;
        private ObservableCollection<WareHouse> _warehouses;

        public ObservableCollection<WareHouse> Warehouses
        {
            get { return _warehouses; }
            set
            {
                if (Equals(value, _warehouses)) return;
                _warehouses = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Vendor> Vendors
        {
            get { return _vendors; }
            set
            {
                if (Equals(value, _vendors)) return;
                _vendors = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Part> MainData
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

        public Part SelectedRow
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