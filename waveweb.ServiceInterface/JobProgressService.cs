using ServiceStack;
using System.Threading.Tasks;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public class JobProgressService : Service
    {
        private readonly IJobProgressProvider jobProgressProvider;

        public JobProgressService(IJobProgressProvider jobProgressProvider)
        {
            this.jobProgressProvider = jobProgressProvider;
        }

        public async Task<JobProgressResponse> Any(JobProgressRequest request)
        {
            return new JobProgressResponse
            {
                Progress = await jobProgressProvider.GetJobProgressAsync(request.JobId)
            };
        }
    }

}
