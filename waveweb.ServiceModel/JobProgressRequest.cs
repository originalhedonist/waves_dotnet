using ServiceStack;
using System;

namespace waveweb.ServiceModel
{
    [Route("/jobprogress")]
    public class JobProgressRequest : IReturn<JobProgress>
    {
        public Guid JobId { get; set; }
    }

    public class JobProgress
    {
        public double Progress { get; set; }
        public bool IsComplete { get; set; }
        public string Message { get; set; }
    }
}
