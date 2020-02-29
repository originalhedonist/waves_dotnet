using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using wavegenerator.library;
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
        [InlineData("Settings_FrequencyLinked.json", "C6CE387BCE819FA67AC23EEE999DA4A22A2248D02C6A33B98448FEAE469A0FD3")]
        [InlineData("Settings_WetnessLinked.json", "5929D9FF329610B930D5316F952F9FFEBED1530376DAA9A3843C0B6DF0DDC30E")]
        [InlineData("Settings_FrequencyIndependent.json", "17FA901DBC00BEE55DCEE6F5FA38F8CCB92BD0F47D458F3FC36533F3069CD609")]
        [InlineData("Settings_WetnessIndependent.json", "1E120A7FB6271590ABAD2F87822BF408B6D7D6E50CA7A63E358D30D5431713E3")]
        public async Task ValidSettings_VerifyFile(string settingsFile, string expectedSha256Hash)
        {
            var sha256 = SHA256.Create();
            await using var memoryStream = new MemoryStream();
            var settings = SettingsLoader.LoadAndValidateSettings(settingsFile);
            var container = DependencyConfig.ConfigureContainer(r => r.AddSingleton(settings));

            var waveStream = container.GetRequiredService<WaveStream>();
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
