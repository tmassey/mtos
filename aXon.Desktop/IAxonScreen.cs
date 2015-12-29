using System;
using System.ComponentModel;
using aXon.Data;
using aXon.Rover.Interfaces;

namespace aXon.Desktop
{
    public interface IAxonScreen: INotifyPropertyChanged
    {
        Guid Id { get; set; }
        string ScreenName { get; set; }
        string ModuleName { get; set; }
        aXonEntities DataService { get; set; }
        MainWindow Shell { get; set; }
    }
}