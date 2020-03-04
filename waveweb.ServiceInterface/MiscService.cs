using Hangfire;
using Microsoft.Extensions.Configuration;
using ServiceStack;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public class MiscService : Service
    {
        private readonly IConfiguration config;
        private readonly IBackgroundJobClient backgroundJobClient;

        public MiscService(IConfiguration config, IBackgroundJobClient backgroundJobClient)
        {
            this.config = config;
            this.backgroundJobClient = backgroundJobClient;
        }
        //Return index.html for unmatched requests so routing is handled on client
        public object Any(FallbackForClientRoutes request) => Request.GetPageResult("/");

        public async Task<TestResponse> Post(TestRequest request)
        {
            var job = backgroundJobClient.Enqueue(() => ALongJob(request, JobCancellationToken.Null));
            
            return new TestResponse { Message = job };
        }

        public static async Task ALongJob(TestRequest data, IJobCancellationToken jobCancellationToken)
        {
            for(int i = 0; i < 10; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
