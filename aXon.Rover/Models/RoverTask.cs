using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using aXon.Rover.Annotations;
using aXon.Rover.Enumerations;
using aXon.TaskTransport;

namespace aXon.Rover.Models
{

    public class RoverTask : INotifyPropertyChanged
    {
        public Guid Id { get; set; }
        public RoverTaskType TaskType { get; set; }
        public TaskStatus Status { get; set; }
        public RoverTrainProperties TrainingProperties { get; set; }
        public RoverExecuteProperties ExecuteionProperties { get; set; }
        

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
       

        
    }
}