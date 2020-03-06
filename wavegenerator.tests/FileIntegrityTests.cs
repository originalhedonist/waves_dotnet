using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using wavegenerator.library;
using wavegenerator.models;
using Xunit;
using Xunit.Abstractions;

namespace wavegenerator.tests
{
    public class FileIntegrityTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public FileIntegrityTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void InvalidSettings()
        {
            Assert.Throws<ValidationException>(() => LoadAndValidateSettings("Settings__Invalid.json"));
        }

        [Theory]
        [InlineData("Settings_FrequencyIndependent.json", "0482ADF98F71289BB2670D868C40E8C6CE8CB8A54ACECCC1D22049BD161DDA1A")]
        [InlineData("Settings_FrequencyIndependent_NoWetnessOnLeft.json", "51EB3981EEB2D40361C1A94182041BF7719A792EB9B815F6E6B21B46F4008648")]
        [InlineData("Settings_FrequencyLinked.json", "FC233821F85D9D9DFE048F4B0934CB17C639E3FD5762C8102B7B60E756327256")]
        [InlineData("Settings_WetnessIndependent.json", "8E48C44288421CCB99BDAE1383DC2743FA418BFE58FAB34DFF75E9B7583F1FD0")]
        [InlineData("Settings_WetnessLinked.json", "42CEEC630E2696D622FE9D5E4CB07D16FB82E16B23EC4A3497E68C74DC30F4EC")]
        public async Task ValidSettings_VerifyFile(string settingsFile, string expectedSha256Hash)
        {
            var sha256 = SHA256.Create();
            await using var memoryStream = new MemoryStream();
            var settings = SettingsLoader.LoadAndValidateSettings(settingsFile);
            var container = DependencyConfig.ConfigureContainer(settings, c =>
            {
                c.AddInstance<IProgressReporter>(new XUnitProgressReporter(testOutputHelper));
            });

            var mp3Stream = container.Resolve<Mp3Stream>();
            await mp3Stream.Write(memoryStream);
            await memoryStream.FlushAsync();
            memoryStream.Seek(0, SeekOrigin.Begin);

            var hash = sha256.ComputeHash(memoryStream);
            var hashString = BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
            Assert.Equal(expectedSha256Hash.Length, hashString.Length);
            Assert.Equal(expectedSha256Hash, hashString);
        }

        private static Settings LoadAndValidateSettings(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var newSettings = JsonConvert.DeserializeObject<Settings>(json);
            Validator.ValidateObject(newSettings, new ValidationContext(newSettings), true);
            return newSettings;
        }
    }
}
