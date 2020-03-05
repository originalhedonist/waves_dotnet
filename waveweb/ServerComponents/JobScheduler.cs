using Hangfire;
using System;
using System.Threading;
using System.Threading.Tasks;
using waveweb.ServiceInterface;
using waveweb.ServiceModel;

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

        public async Task<Guid> ScheduleJob<TService, TData>(TData data) where TService : ILongJobProcessor<TData>
        {
            var jobId = Guid.NewGuid();
            backgroundJobClient.Enqueue<TService>(s => s.Run(data, jobId, CancellationToken.None));
            await jobProgressProvider.SetJobProgressAsync(jobId, new JobProgress { IsComplete = false, Progress = 0 });
            // which will get set to min of 5%
            return jobId;
        }
    }
}
