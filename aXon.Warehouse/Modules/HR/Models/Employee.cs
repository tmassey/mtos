using System;
using System.Collections.ObjectModel;
using aXon.Warehouse.Modules.HR.Enumerations;
using aXon.Warehouse.Modules.Security;
using aXon.Warehouse.Modules.Security.Models;

namespace aXon.Warehouse.Modules.HR.Models
{
    public class Employee:Person
    {
        private ObservableCollection<Guid> _benifits;
        private Guid _employeeTypeId;
        private PayType _payType;
        private decimal _currentPayRate;
        private Guid _positionId;
        private string _employeeNum;
        private string _ssn;

        public string SSN
        {
            get { return _ssn; }
            set
            {
                if (value == _ssn) return;
                _ssn = value;
                OnPropertyChanged();
            }
        }

        public string EmployeeNum
        {
            get { return _employeeNum; }
            set
            {
                if (value == _employeeNum) return;
                _employeeNum = value;
                OnPropertyChanged();
            }
        }

        public Guid PositionId
        {
            get { return _positionId; }
            set
            {
                if (value.Equals(_positionId)) return;
                _positionId = value;
                OnPropertyChanged();
            }
        }

        public decimal CurrentPayRate
        {
            get { return _currentPayRate; }
            set
            {
                if (value == _currentPayRate) return;
                _currentPayRate = value;
                OnPropertyChanged();
            }
        }

        public PayType PayType
        {
            get { return _payType; }
            set
            {
                if (value == _payType) return;
                _payType = value;
                OnPropertyChanged();
            }
        }

        public Guid EmployeeTypeId
        {
            get { return _employeeTypeId; }
            set
            {
                if (value.Equals(_employeeTypeId)) return;
                _employeeTypeId = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Guid> Benifits
        {
            get { return _benifits; }
            set
            {
                if (Equals(value, _benifits)) return;
                _benifits = value;
                OnPropertyChanged();
            }
        }
    }
}
