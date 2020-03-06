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

        public async Task ScheduleJob<TService, TData>(Guid jobId, TData data) where TService : ILongJobProcessor<TData>
        {
            backgroundJobClient.Enqueue<TService>(s => s.Run(data, jobId, CancellationToken.None));
            await jobProgressProvider.SetJobProgressAsync(jobId, new JobProgress { IsComplete = false, Progress = 0, Message = "Starting..." });
            // which will get set to min of 5%
        }
    }
}
