using System;
using aXon.TaskTransport;


namespace aXon.Worker.EventArgs
{
    public class OnProgressArgs
    {
        public Guid TaskId { get; set; }
        public DateTime StartTime { get; set; }

        public DateTime CurrentTime { get; set; }

        public decimal PercentComplete { get; set; }

        public TaskStatus Status { get; set; }
    }
}