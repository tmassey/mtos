using System;

namespace aXon.Worker.EventArgs
{
    public class OnErrorArgs
    {
        public Guid TaskId { get; set; }
        public string Error { get; set; }
    }
}