using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aXon.Rover.Enumerations;
using aXon.Rover.Models;

namespace aXon.Warehouse.Modules.Robotics.Robot.Models
{
    public class Robot:CompanyBaseModel
    {
        public string SerialNumber { get;set; }
        public Position CurrentLocation { get; set; }
        public RoverMode CurrentMode { get; set; }

    }

    public enum RobotJobType
    {

    }

    public class RobotJob : CompanyBaseModel
    {
        public RobotJobType JobType { get; set; }
        
    }
}
