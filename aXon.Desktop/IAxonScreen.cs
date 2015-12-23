using System;
using System.ComponentModel;
using aXon.Rover.Interfaces;

namespace aXon.Desktop
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