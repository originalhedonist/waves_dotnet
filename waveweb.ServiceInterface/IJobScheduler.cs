using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace waveweb.ServiceInterface
{
    public interface IJobScheduler
    {
        Task<Guid> ScheduleJob<TService, TData>(TData data) where TService : ILongJobProcessor<TData>;
    }

    public interface ILongJobProcessor<TData>
    {
        Task Run(TData data, Guid jobId, CancellationToken cancellationToken);
    }
}