﻿using System;
using System.Threading.Tasks;
using wavegenerator.library;
using wavegenerator.models;
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

        public async Task ReportProgress(double progress, JobProgressStatus status, string message)
        {
            if(lastReport == null || status != JobProgressStatus.InProgress || DateTime.Now.Subtract(lastReport.Value) > TimeSpan.FromSeconds(5))
            {
                lastReport = DateTime.Now;
                await this.jobProgressProvider.SetJobProgressAsync(this.jobId,
                    new JobProgress
                    {
                        Progress = progress,
                        Status = status,
                        Message = message
                    });
            }
        }
    }
}
