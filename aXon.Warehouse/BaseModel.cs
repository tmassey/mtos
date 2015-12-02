using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using aXon.Rover.Annotations;

namespace aXon.Warehouse
{
    public class BaseModel:INotifyPropertyChanged
    {
        private Guid _id;
        private bool _isActiveRecord;
        private Guid _createdBy;
        private Guid _modifiedBy;
        private DateTime _lastEditDateTime;
        private DateTime _createDateTime;

        public Guid Id
        {
            get { return _id; }
            set
            {
                if (value.Equals(_id)) return;
                _id = value;
                OnPropertyChanged();
            }
        }

        public DateTime CreateDateTime
        {
            get { return _createDateTime; }
            set
            {
                if (value.Equals(_createDateTime)) return;
                _createDateTime = value;
                OnPropertyChanged();
            }
        }

        public DateTime LastEditDateTime
        {
            get { return _lastEditDateTime; }
            set
            {
                if (value.Equals(_lastEditDateTime)) return;
                _lastEditDateTime = value;
                OnPropertyChanged();
            }
        }

        public Guid ModifiedBy
        {
            get { return _modifiedBy; }
            set
            {
                if (value.Equals(_modifiedBy)) return;
                _modifiedBy = value;
                OnPropertyChanged();
            }
        }

        public Guid CreatedBy
        {
            get { return _createdBy; }
            set
            {
                if (value.Equals(_createdBy)) return;
                _createdBy = value;
                OnPropertyChanged();
            }
        }

        public bool IsActiveRecord
        {
            get { return _isActiveRecord; }
            set
            {
                if (value == _isActiveRecord) return;
                _isActiveRecord = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
