﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aXon.Data;

namespace aXon.Desktop.ViewModels.Modules.Robotics
{
    public class RobotsViewModel : BaseViewModel
    {
        private ObservableCollection<aXonRobot> _mainData;
        private aXonRobot _selectedRow;
        private bool _editMode;
        

        public ObservableCollection<aXonRobot> MainData
        {
            get { return _mainData; }
            set
            {
                if (Equals(value, _mainData)) return;
                _mainData = value;
                OnPropertyChanged();
            }
        }

      

        public aXonRobot SelectedRow
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