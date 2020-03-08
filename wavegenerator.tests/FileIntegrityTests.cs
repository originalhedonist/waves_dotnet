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
        [InlineData("Settings_FrequencyIndependent.json", "D4DB906BC43671F3A535CB70FF648E98D4AA5E732AF9459811B18F53AFE86F96")]
        [InlineData("Settings_FrequencyIndependent_NoWetnessOnLeft.json", "7BB0A7E63E387A14676CD35B4A3B1EAE6D717F710333CAAC9B4E148A84803EDA")]
        [InlineData("Settings_FrequencyLinked.json", "D6242B11A5EE12549C4ECCC82DFCD7A7D8F08C1E172D33395137CDBEED4FC56C")]
        [InlineData("Settings_WetnessIndependent.json", "8E48C44288421CCB99BDAE1383DC2743FA418BFE58FAB34DFF75E9B7583F1FD0")]
        [InlineData("Settings_WetnessLinked.json", "C47B459E44ACF965FD1C54AD17C56908466B3C980EC7362F4746D823836C283A")]

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
            if(expectedSha256Hash != hashString)
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                await using var fileStream = new FileStream(settingsFile + ".mp3", FileMode.Create, FileAccess.Write);
                await memoryStream.CopyToAsync(fileStream);
            }
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
