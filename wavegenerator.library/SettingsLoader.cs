using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using wavegenerator.models;

namespace wavegenerator.library
{
    public static class SettingsLoader
    {
        public static async Task<SettingsCommon> LoadAndValidateSettings(string filePath)
        {
            string json = await File.ReadAllTextAsync(filePath);
            var newSettingsBase = JsonConvert.DeserializeObject<SettingsCommon>(json);
            var version = newSettingsBase.Version ?? 1;
            SettingsCommon newSettings;
            if(version == 1)
            {
                newSettings = JsonConvert.DeserializeObject<Settings>(json);
            }
            else if (version == 2)
            {
                newSettings = JsonConvert.DeserializeObject<SettingsV2>(json);
            }
            else
            {
                throw new InvalidOperationException($"Version {version} not recognized");
            }
            Validator.ValidateObject(newSettings, new ValidationContext(newSettings), true);
            return newSettings;
        }
    }
}
