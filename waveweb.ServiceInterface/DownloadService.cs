using Microsoft.Extensions.Logging;
using ServiceStack;
using ServiceStack.Web;
using System;
using System.Collections.Generic;
using System.IO;
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

        public Stream Get(DownloadFileRequest request)
        {
            string filePath = Path.Combine(DownloadDir, request.Id.ToString());
            return new DownloadFileResult($"{DateTime.Now:yyyyMMdd_HHmmss}.txt", filePath);
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

    public class DownloadFileResult : FileStream, IHasOptions
    {
        private readonly string downloadName;

        public DownloadFileResult(string downloadName, string fullPath) :
            base(fullPath, FileMode.Open, FileAccess.Read)
        {
            this.downloadName = downloadName;
        }

        public IDictionary<string, string> Options => new Dictionary<string, string>
        {
            { "Content-Disposition", $"attachment;filename={downloadName}" }
        };
    }


}
