using Microsoft.Extensions.Configuration;
using ServiceStack;
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
            var jobId = await jobScheduler.ScheduleJob<TestLongJob, long>(request.Chunks);
            return new TestResponse { Message = "The job has started, and is running.", JobId = jobId };
        }

    }
}
