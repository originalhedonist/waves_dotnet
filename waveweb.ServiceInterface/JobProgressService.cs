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

        public Task<JobProgress> Any(JobProgressRequest request)
        {
            return jobProgressProvider.GetJobProgressAsync(request.JobId);
        }
    }
}
