using Hangfire;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ultimate.DI;
using wavegenerator.library;
using wavegenerator.models;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public class FileCreator : ILongJobProcessor<Settings>
    {
        private readonly IUltimateContainerProvider containerProvider;
        private readonly IJobProgressProvider jobProgressProvider;
        private readonly ILogger<FileCreator> logger;
        private readonly IOutputDirectoryProvider outputDirectoryProvider;

        public FileCreator(IUltimateContainerProvider containerProvider, IJobProgressProvider jobProgressProvider, ILogger<FileCreator> logger, IOutputDirectoryProvider outputDirectoryProvider)
        {
            this.containerProvider = containerProvider;
            this.jobProgressProvider = jobProgressProvider;
            this.logger = logger;
            this.outputDirectoryProvider = outputDirectoryProvider;
        }

        [AutomaticRetry(Attempts = 0)]
        public async Task Run(Settings data, Guid jobId, CancellationToken cancellationToken)
        {
            try
            {
                // set up the stack
                var container = containerProvider.GetFullFeatureContainer(data);
                container.AddInstance<IJobIdProvider>(new JobIdProvider { JobId = jobId });
                container.AddInstance<Settings>(data);
                container.AddInstance<IWaveFileMetadata>(data);
                var mp3Stream = container.Resolve<Mp3Stream>();

                // prepare for the file creation                
                var fullPath = Path.Combine(outputDirectoryProvider.GetOutputDirectory(), $"{jobId}.mp3");

                var rowsInserted = await jobProgressProvider.SetJobProgressAsync(jobId, new JobProgress { Status = JobProgressStatus.InProgress, Progress = 0, Message = "Starting..." });
                if (rowsInserted == 1)
                    //this check is necessary so it doesn't restart old jobs if azure kills the service.
                    //otherwise, hangfire would kick in when it starts again and start processing them all and it
                    //would keep exceeding its quota again.
                {
                    logger.LogInformation("Inserted a row - writing file!");
                    // write it
                    await mp3Stream.Write(fullPath);
                }
                else logger.LogInformation("Didn't insert a row - not writing file");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in FileCreator.Run");
                throw;
            }
        }
    }
}
