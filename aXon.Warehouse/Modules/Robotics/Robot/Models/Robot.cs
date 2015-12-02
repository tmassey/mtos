using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aXon.Rover.Enumerations;
using aXon.Rover.Models;

namespace aXon.Warehouse.Modules.Robotics.Robot.Models
{
    public class Robot:BaseModel
    {
        public int SerialNumber { get;set; }
        public Position CurrentLocation { get; set; }
        public RoverMode CurrentMode { get; set; }

    }
}
