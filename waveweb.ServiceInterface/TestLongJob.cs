using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public class TestLongJob : ILongJobProcessor<long>
    {
        private readonly IJobProgressProvider jobProgressProvider;

        public TestLongJob(IJobProgressProvider jobProgressProvider)
        {
            this.jobProgressProvider = jobProgressProvider;
        }

        public async Task Run(long data, Guid jobId, CancellationToken cancellationToken)
        {
            for (long i = 0; i < data; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                await jobProgressProvider.SetJobProgressAsync(jobId,
                    new JobProgress
                    {
                        Progress = (double)i / data,
                        IsComplete = false,
                        Message = $"Done {i} chunks"
                    });
            }
            if(!Directory.Exists(DownloadService.DownloadDir))
            {
                Directory.CreateDirectory(DownloadService.DownloadDir);
            }
            await File.WriteAllTextAsync($"{DownloadService.DownloadDir}/{jobId}", "hello from the long job!");
            await jobProgressProvider.SetJobProgressAsync(jobId, new JobProgress { IsComplete = true, Message = "Long job finished!" });
        }
    }

}
