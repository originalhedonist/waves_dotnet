using Microsoft.Extensions.Logging;
using ServiceStack;
using System.IO;
using System.Threading.Tasks;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public class UploadSettingsService : Service
    {
        private readonly ILogger<UploadSettingsService> logger;

        public UploadSettingsService(ILogger<UploadSettingsService> logger)
        {
            this.logger = logger;
        }

        public async Task Post(UploadSettingsRequest uploadSettingsRequest)
        {
            
            logger.LogInformation($"Received settings, files = {Request.Files.Length}");
        }
    }
}
