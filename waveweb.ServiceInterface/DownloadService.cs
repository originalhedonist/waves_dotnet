using Microsoft.Extensions.Logging;
using ServiceStack;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using wavegenerator.library;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public class DownloadService : Service
    {
        public DownloadService(ILogger<DownloadService> logger, IOutputDirectoryProvider outputDirectoryProvider)
        {
            this.logger = logger;
            this.outputDirectoryProvider = outputDirectoryProvider;
        }

        private readonly ILogger<DownloadService> logger;
        private readonly IOutputDirectoryProvider outputDirectoryProvider;

        public async Task<Stream> Get(DownloadFileRequest request)
        {
            var filePath = Directory.GetFiles(outputDirectoryProvider.GetOutputDirectory(), $"{request.Id}.*").FirstOrDefault();
            if (filePath == null)
            {
                return await NotFoundFileResult.Create();
            }
            else
            {
                var extension = Path.GetExtension(filePath);
                var downloadName = $"{DateTime.Now:yyyyMMdd_HHmmss}{extension}";
                return new DownloadFileResult(downloadName, filePath);
            }
            
        }
    }
}
