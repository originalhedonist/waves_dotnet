using System;
using System.Threading.Tasks;

namespace waveweb.ServiceInterface
{
    public interface IJobProgressProvider
    {
        Task SetJobProgressAsync(Guid jobId, double progress);
        Task<double> GetJobProgressAsync(Guid jobId);
    }
}