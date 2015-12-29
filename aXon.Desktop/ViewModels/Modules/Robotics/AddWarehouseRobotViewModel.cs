using System;
using System.Collections.ObjectModel;
using aXon.Data;

namespace aXon.Desktop.ViewModels.Modules.Robotics
{
    public class AddWarehouseRobotViewModel : BaseViewModel
    {
        private string _registrationKey4;
        private string _registrationKey3;
        private string _registrationKey2;
        private string _registrationKey1;
        private string _registrationKey;
        private string _serialNumber;
        private WarehouseRobot _robot;
        private ObservableCollection<WareHouse> _warehouses;
        private Guid _wareHouseId;
        private int _x;
        private int _y;

        public Guid WarehouseId
        {
            get { return _wareHouseId; }
            set
            {
                if (value.Equals(_wareHouseId)) return;
                _wareHouseId = value;
                OnPropertyChanged();
            }
        }

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

        public int X
        {
            get { return _x; }
            set
            {
                if (value == _x) return;
                _x = value;
                OnPropertyChanged();
            }
        }

        public int Y
        {
            get { return _y; }
            set
            {
                if (value == _y) return;
                _y = value;
                OnPropertyChanged();
            }
        }

        public WarehouseRobot Robot
        {
            get { return _robot; }
            set
            {
                if (Equals(value, _robot)) return;
                _robot = value;
                OnPropertyChanged();
            }
        }

        public string SerialNumber
        {
            get { return _serialNumber; }
            set
            {
                if (value == _serialNumber) return;
                _serialNumber = value;
                OnPropertyChanged();
            }
        }

        public string RegistrationKey
        {
            get { return _registrationKey; }
            set
            {
                if (value == _registrationKey) return;
                _registrationKey = value;
                OnPropertyChanged();
            }
        }

        public string RegistrationKey1
        {
            get { return _registrationKey1; }
            set
            {
                if (value == _registrationKey1) return;
                _registrationKey1 = value;
                OnPropertyChanged();
            }
        }

        public string RegistrationKey2
        {
            get { return _registrationKey2; }
            set
            {
                if (value == _registrationKey2) return;
                _registrationKey2 = value;
                OnPropertyChanged();
            }
        }

        public string RegistrationKey3
        {
            get { return _registrationKey3; }
            set
            {
                if (value == _registrationKey3) return;
                _registrationKey3 = value;
                OnPropertyChanged();
            }
        }

        public string RegistrationKey4
        {
            get { return _registrationKey4; }
            set
            {
                if (value == _registrationKey4) return;
                _registrationKey4 = value;
                OnPropertyChanged();
            }
        }
    }
}