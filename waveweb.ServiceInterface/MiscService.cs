using Microsoft.Extensions.Configuration;
using ServiceStack;
using System;
using System.Threading.Tasks;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public class MiscService : Service
    {
        private readonly IJobScheduler jobScheduler;

        public MiscService(IJobScheduler jobScheduler)
        {
            this.jobScheduler = jobScheduler;
        }

        //Return index.html for unmatched requests so routing is handled on client
        public object Any(FallbackForClientRoutes request) => Request.GetPageResult("/");

        public async Task<TestResponse> Post(TestRequest request)
        {
            var jobId = Guid.NewGuid();
            await jobScheduler.ScheduleJob<TestLongJob, long>(jobId, request.Chunks);
            return new TestResponse { Message = "The job has started, and is running.", JobId = jobId };
        }

    }
}
