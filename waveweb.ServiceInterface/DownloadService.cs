using Microsoft.Extensions.Logging;
using ServiceStack;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public class DownloadService : Service
    {
        public DownloadService(ILogger<DownloadService> logger)
        {
            this.logger = logger;
        }

        public const string DownloadDir = "DownloadableFiles";
        private readonly ILogger<DownloadService> logger;

        public async Task<Stream> Get(DownloadFileRequest request)
        {
            var filePath = Directory.GetFiles(DownloadDir, $"{request.Id}.*").FirstOrDefault();
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

        public Stream Get(TestDownloadRequest request)
        {
            try
            {
                var guid = Guid.NewGuid();

                string filePath = Path.Combine(DownloadDir, guid.ToString());
                if (!Directory.Exists(DownloadDir))
                {
                    Directory.CreateDirectory(DownloadDir);
                }
                File.WriteAllText(filePath, "hello from test download service.");
                return new DownloadFileResult("testfile.txt", filePath);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing TestDownloadRequest");
                throw;
            }
        }
    }
}
