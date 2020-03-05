using System;
using System.Threading.Tasks;
using wavegenerator.library;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public class WebProgressReporter : IProgressReporter
    {
        private readonly IJobProgressProvider jobProgressProvider;
        private readonly Guid jobId;
        private DateTime? lastReport = null;

        public WebProgressReporter(IJobIdProvider jobIdProvider, IJobProgressProvider jobProgressProvider)
        {
            this.jobProgressProvider = jobProgressProvider;
            this.jobId = jobIdProvider.JobId;
        }

        public async Task ReportProgress(double progress, bool complete)
        {
            if(lastReport == null || DateTime.Now.Subtract(lastReport.Value) > TimeSpan.FromSeconds(30))
            {
                lastReport = DateTime.Now;
                await this.jobProgressProvider.SetJobProgressAsync(this.jobId,
                    new JobProgress
                    {
                        Progress = progress,
                        IsComplete = complete
                    });
            }
        }
    }


}
