using Hangfire;
using System;
using System.Threading;
using System.Threading.Tasks;
using waveweb.ServiceInterface;

namespace waveweb.ServerComponents
{
    public class JobScheduler : IJobScheduler
    {
        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly IJobProgressProvider jobProgressProvider;

        public JobScheduler(IBackgroundJobClient backgroundJobClient, IJobProgressProvider jobProgressProvider)
        {
            this.backgroundJobClient = backgroundJobClient;
            this.jobProgressProvider = jobProgressProvider;
        }

        public Task ScheduleJob<TService, TData>(Guid jobId, TData data) where TService : ILongJobProcessor<TData>
        {
            backgroundJobClient.Enqueue<TService>(s => s.Run(data, jobId, CancellationToken.None));
            return Task.CompletedTask;
        }
    }
}
