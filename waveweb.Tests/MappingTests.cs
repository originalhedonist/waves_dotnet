using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Newtonsoft.Json;
using NUnit.Framework;
using wavegenerator.library;
using wavegenerator.models;
using waveweb.ServiceInterface;
using waveweb.ServiceModel;

namespace waveweb.Tests
{
    public class MappingTests
    {
        [Test]
        public void ConfigurationIsValid()
        {
            var mappingConfig = Mapping.CreateMapperConfiguration();
            mappingConfig.AssertConfigurationIsValid();
        }

        [Test]
        public void RoundTrip()
        {
            var settings = SettingsLoader.LoadAndValidateSettings("Settings_RoundTrip.json");
            var settingsJson = JsonConvert.SerializeObject(settings); // convert back to json, to avoid whitespace issues, etc

            var mappingConfig = Mapping.CreateMapperConfiguration();
            var mapper = new Mapper(mappingConfig);

            var createFileRequest = mapper.Map<Settings, CreateFileRequest>(settings);
            var roundTrippedSettings = mapper.Map<CreateFileRequest, Settings>(createFileRequest);

            var roundTrippedSettingsJson = JsonConvert.SerializeObject(roundTrippedSettings);

            Assert.AreEqual(settingsJson, roundTrippedSettingsJson);
        }
    }
}
