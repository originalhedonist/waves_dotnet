using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;
using wavegenerator.library;
using wavegenerator.models;
using waveweb.ServiceInterface;
using waveweb.ServiceModel;
using Xunit;

namespace waveweb.Tests
{
    public class MappingTests
    {
        [Fact]
        public void ConfigurationIsValid()
        {
            var mappingConfig = Mapping.CreateMapperConfiguration();
            mappingConfig.AssertConfigurationIsValid();
        }

        [Fact]
        public async Task RoundTrip()
        {
            var settings = await SettingsLoader.LoadAndValidateSettings("Settings_RoundTrip.json");
            var settingsv1 = Assert.IsAssignableFrom<Settings>(settings);
            var settingsJson = JsonConvert.SerializeObject(settings); // convert back to json, to avoid whitespace issues, etc

            var mappingConfig = Mapping.CreateMapperConfiguration();
            var mapper = new Mapper(mappingConfig);

            var createFileRequest = mapper.Map<Settings, CreateFileRequest>(settingsv1);
            var roundTrippedSettings = mapper.Map<CreateFileRequest, Settings>(createFileRequest);

            var roundTrippedSettingsJson = JsonConvert.SerializeObject(roundTrippedSettings);

            Assert.Equal(settingsJson, roundTrippedSettingsJson);
        }
    }
}
