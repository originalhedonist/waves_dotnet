using AutoMapper;
using Newtonsoft.Json;
using ServiceStack;
using wavegenerator.models;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public class UploadSettingsService : Service
    {
        private readonly IMapper mapper;

        public UploadSettingsService(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public UploadSettingsResponse Post(UploadSettingsRequest uploadSettingsRequest)
        {
            var settings = JsonConvert.DeserializeObject<Settings>(uploadSettingsRequest.SettingsFile);
            var createFileRequest = mapper.Map<Settings, CreateFileRequest>(settings);
            return new UploadSettingsResponse { Request = createFileRequest };
        }
    }
}
