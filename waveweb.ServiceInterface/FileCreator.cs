using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using wavegenerator.library;
using wavegenerator.models;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public class FileCreator : ILongJobProcessor<Settings>
    {
        private readonly IFullFeatureUltimateContainerProvider containerProvider;
        private readonly IJobProgressProvider jobProgressProvider;
        private readonly ILogger<FileCreator> logger;

        public FileCreator(IFullFeatureUltimateContainerProvider containerProvider, IJobProgressProvider jobProgressProvider, ILogger<FileCreator> logger)
        {
            this.containerProvider = containerProvider;
            this.jobProgressProvider = jobProgressProvider;
            this.logger = logger;
        }

        public async Task Run(Settings data, Guid jobId, CancellationToken cancellationToken)
        {
            try
            {
                // set up the stack
                var ultimateContainer = containerProvider.GetContainer();
                var nestedContainer = ultimateContainer.GetNestedContainer();
                nestedContainer.AddInstance<IJobIdProvider>(new JobIdProvider { JobId = jobId });
                nestedContainer.AddInstance<Settings>(data);
                var mp3Stream = nestedContainer.Resolve<Mp3Stream>();

                // prepare for the file creation
                if (!Directory.Exists(DownloadService.DownloadDir))
                {
                    Directory.CreateDirectory(DownloadService.DownloadDir);
                }
                var fullPath = Path.Combine(DownloadService.DownloadDir, jobId.ToString());

                // write it
                await mp3Stream.Write(fullPath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error during file creation");
                await jobProgressProvider.SetJobProgressAsync(jobId, new JobProgress { IsComplete = true, Message = "File creation finished. Errors occurred." });
                throw;
            }
        }
    }
}
