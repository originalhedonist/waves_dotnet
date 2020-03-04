using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using wavegenerator.library;
using wavegenerator.models;
using Xunit;

namespace wavegenerator.tests
{
    public class FileIntegrityTests
    {
        [Fact]
        public void InvalidSettings()
        {
            Assert.Throws<ValidationException>(() => LoadAndValidateSettings("Settings__Invalid.json"));
        }

        [Theory]
        [InlineData("Settings_FrequencyIndependent.json", "3A5DEDE8294960BA1A6472C0C13AA503A134A5FBBBA8D88E3282F9F6E7DC2043")]
        [InlineData("Settings_FrequencyIndependent_NoWetnessOnLeft.json", "6C25F302AE26F2F504C51151306ECC7D3BCA0314EF371652BA28A10956174BFA")]
        [InlineData("Settings_FrequencyLinked.json", "BD3E67599EB036294138B0F6C2A70C591B556160BF094F863E58B285F1C65C7D")]
        [InlineData("Settings_WetnessIndependent.json", "7EA35E6A26AB060A9BC63F24467F6EB1C1A054BD3416DE99EAFD40E8B3E2366E")]
        [InlineData("Settings_WetnessLinked.json", "DF4B8BF608E9F43F059A9D85B4223B03981A28CFF19B012EC41AB97DB4FA2BF2")]
        public async Task ValidSettings_VerifyFile(string settingsFile, string expectedSha256Hash)
        {
            var sha256 = SHA256.Create();
            await using var memoryStream = new MemoryStream();
            var settings = SettingsLoader.LoadAndValidateSettings(settingsFile);
            var container = DependencyConfig.ConfigureContainer(settings);

            var waveStream = container.Resolve<WaveStream>();
            await waveStream.Write(memoryStream);
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
