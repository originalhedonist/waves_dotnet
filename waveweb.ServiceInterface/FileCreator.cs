using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using wavegenerator.library;
using wavegenerator.models;

namespace waveweb.ServiceInterface
{
    public class FileCreator : ILongJobProcessor<Settings>
    {
        private readonly IFullFeatureUltimateContainerProvider containerProvider;

        public FileCreator(IFullFeatureUltimateContainerProvider containerProvider)
        {
            this.containerProvider = containerProvider;
        }

        public async Task Run(Settings data, Guid jobId, CancellationToken cancellationToken)
        {
            // set up the stack
            var ultimateContainer = containerProvider.GetContainer();
            var nestedContainer = ultimateContainer.GetNestedContainer();
            nestedContainer.AddInstance<IJobIdProvider>(new JobIdProvider { JobId = jobId });
            nestedContainer.AddInstance<Settings>(data);
            var waveStream = nestedContainer.Resolve<WaveStream>();

            // prepare for the file creation
            if(!Directory.Exists(DownloadService.DownloadDir))
            {
                Directory.CreateDirectory(DownloadService.DownloadDir);
            }
            var fullPath = Path.Combine(DownloadService.DownloadDir, jobId.ToString());

            // write it
            await waveStream.Write(fullPath);
        }
    }
}
