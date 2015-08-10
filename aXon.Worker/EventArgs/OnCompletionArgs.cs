using System;

namespace aXon.Worker.EventArgs
{
    public class OnCompletionArgs
    {
        public Guid TaskId { get; set; }
        public long Totaltime { get; set; }
        public long Average { get; set; }
        public string Results { get; set; }
    }
}