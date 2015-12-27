using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aXon.Desktop.ViewModels
{
    public class MainViewModel: BaseViewModel
    {
        private string _warehouseName;

        public string WarehouseName
        {
            get { return _warehouseName; }
            set
            {
                if (value == _warehouseName) return;
                _warehouseName = value;
                OnPropertyChanged();
            }
        }
    }
}
