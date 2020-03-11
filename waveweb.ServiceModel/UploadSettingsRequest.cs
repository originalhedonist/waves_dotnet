using ServiceStack;
using ServiceStack.Web;
using System.IO;

namespace waveweb.ServiceModel
{
    [Route("/uploadsettings")]
    public class UploadSettingsRequest : IReturn<UploadSettingsResponse>
    {
        public string SettingsFile { get; set; }
    }

    public class UploadSettingsResponse
    {

    }
}
