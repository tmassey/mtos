using System;
using aXon.TaskTransport.Interfaces;

namespace aXon.TaskTransport.Messages
{
    public class RobotJobMessage : IBaseMessage
    {

        #region IBaseMessage implementation

        public Guid MessageId { get; set; }

        public DateTime TransmisionDateTime { get; set; }

        #endregion

        public Guid JobId { get; set; }
        public Guid NetworkId { get; set; }
        public string RobotSerial { get; set; }
       
    }
}