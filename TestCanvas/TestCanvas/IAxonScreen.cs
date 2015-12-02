using System;
using System.ComponentModel;
using aXon.Rover.Interfaces;
using aXon.Warehouse.Desktop;

namespace aXon.Warehouse.Enumerations
{
    public interface IAxonScreen: INotifyPropertyChanged
    {
        Guid Id { get; set; }
        string ScreenName { get; set; }
        string ModuleName { get; set; }
        IDataService DataService { get; set; }
        MainWindow Shell { get; set; }
    }
}