using ServiceStack;

namespace waveweb.ServiceModel
{
    [Route("/uploadsettings")]
    public class UploadSettingsRequest : IReturn<UploadSettingsResponse>
    {
        public string SettingsFile { get; set; }
    }
}
