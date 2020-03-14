using System;
using System.Threading.Tasks;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public interface IJobProgressProvider
    {
        Task<int> SetJobProgressAsync(Guid jobId, JobProgress jobProgress);
        Task<JobProgress> GetJobProgressAsync(Guid jobId);
    }
}
