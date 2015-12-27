using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aXon.Desktop.ViewModels.Modules.Settings
{
    public class NeuralNetworksViewModel:BaseViewModel
    {
        private ObservableCollection<WarehouseNeuralNetwork> _mainData;
        private WarehouseNeuralNetwork _selectedRow;
        private bool _editMode;
        private ObservableCollection<Company> _companies;

        public ObservableCollection<WarehouseNeuralNetwork> MainData
        {
            get { return _mainData; }
            set
            {
                if (Equals(value, _mainData)) return;
                _mainData = value;
                OnPropertyChanged();
            }
        }

      

        public WarehouseNeuralNetwork SelectedRow
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
