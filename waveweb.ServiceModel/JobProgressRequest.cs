using ServiceStack;
using System;

namespace waveweb.ServiceModel
{
    [Route("/jobprogress")]
    public class JobProgressRequest : IReturn<JobProgressResponse>
    {
        public Guid JobId { get; set; }
    }
}
