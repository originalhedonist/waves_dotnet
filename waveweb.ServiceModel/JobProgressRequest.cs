using ServiceStack;
using System;
using wavegenerator.models;

namespace waveweb.ServiceModel
{
    [Route("/jobprogress")]
    public class JobProgressRequest : IReturn<JobProgress>
    {
        public Guid JobId { get; set; }
    }

    public class JobProgress
    {
        public JobProgressStatus Status { get; set; }
        public double Progress { get; set; }
        public string Message { get; set; }
    }
}
