using ServiceStack;
using System.IO;

namespace waveweb.ServiceModel
{
    [Route("/downloadsettings")]
    public class DownloadSettingsRequest : IReturn<DownloadSettingsResponse>
    {
        public CreateFileRequest Request { get; set; }
    }
}
