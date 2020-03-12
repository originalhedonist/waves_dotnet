﻿using Hangfire;
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
                var container = containerProvider.GetFullFeatureContainer();
                container.AddInstance<IJobIdProvider>(new JobIdProvider { JobId = jobId });
                container.AddInstance<Settings>(data);
                container.AddInstance<IWaveFileMetadata>(data);
                var mp3Stream = container.Resolve<Mp3Stream>();

                // prepare for the file creation                
                var fullPath = Path.Combine(outputDirectoryProvider.GetOutputDirectory(), $"{jobId}.mp3");

                // write it
                await mp3Stream.Write(fullPath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in FileCreator.Run");
                throw;
            }
        }
    }
}
