using System;

namespace aXon.Worker
{
    public class BenchmarkResult
    {
        public Guid Id { get; set; }
        public long Totaltime { get; set; }
        public long Average { get; set; }
    }
}