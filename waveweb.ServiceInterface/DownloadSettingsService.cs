using AutoMapper;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.IO;
using System.Threading.Tasks;
using wavegenerator.library;
using wavegenerator.models;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public class DownloadSettingsService : Service
    {
        private readonly IMapper mapper;
        private readonly IOutputDirectoryProvider outputDirectoryProvider;

        public DownloadSettingsService(IMapper mapper, IOutputDirectoryProvider outputDirectoryProvider)
        {
            this.mapper = mapper;
            this.outputDirectoryProvider = outputDirectoryProvider;
        }

        public async Task<DownloadSettingsResponse> Post(DownloadSettingsRequest downloadSettingsRequest)
        {
            var settings = mapper.Map<CreateFileRequest, Settings>(downloadSettingsRequest.Request);
            var downloadFileId = Guid.NewGuid();
            await using var fileStream = File.OpenWrite(Path.Combine(outputDirectoryProvider.GetOutputDirectory(), $"{downloadFileId}.json"));
            var jsonSerializer = new JsonSerializer { Formatting = Formatting.Indented };
            await using var textWriter = new StreamWriter(fileStream);
            jsonSerializer.Serialize(textWriter, settings);
            return new DownloadSettingsResponse { DownloadId = downloadFileId };
        }
    }
}
