using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace wavegenerator
{
    public class Program
    {
        private static bool hasLame;

        public static async Task Main(string[] args)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                if (args.Length == 0)
                {
                    await Console.Out.WriteLineAsync($"No settings file passed.\nPlease copy and modify one of the example settings files to <name>.settings.json, then pass the modified file to the program on the command line.");
                }
                else if(args.Length > 2)
                {
                    throw new InvalidOperationException($"Too many files passed. Please only pass one file.");
                }
                else
                {
                    var filePath = args.Single();

                    Settings.Instance = LoadAndValidateSettings(filePath);

                    hasLame = Settings.Instance.ConvertToMp3 && TestForLame();
                    var tasks = Enumerable.Range(0, Settings.Instance.NumFiles)
                        .Select(i => WriteFile(i))
                        .ToArray();
                    await Task.WhenAll(tasks);

                    stopwatch.Stop();
                    ConsoleWriter.WriteLine($"{tasks.Length} file(s) successfully created in {stopwatch.Elapsed}", ConsoleColor.Green);
                }
            }
            catch (Exception ex)
            {
                ConsoleWriter.WriteLine(ex.Message, ConsoleColor.Red);
            }
        }

        private static Settings LoadAndValidateSettings(string filePath)
        {
            string json = File.ReadAllText(filePath);
            var newSettings = JsonConvert.DeserializeObject<Settings>(json);
            Validator.ValidateObject(newSettings, new ValidationContext(newSettings), true);
            return newSettings;
        }

        private static async Task WriteFile(int uniqueifier)
        {
            var compositionName = $"{GetRandomName()}_{DateTime.Now.ToString("yyyyMMdd_HHmm")}_{uniqueifier}";
            var patterns = Settings.Instance.ChannelSettings.Select(c => new RiseApplier(c.Rises, new PulseGenerator(c))).ToArray();
            var carrierFrequencyApplier = new CarrierFrequencyApplier(patterns);

            await File.WriteAllTextAsync($"{compositionName}.parameters.json", JsonConvert.SerializeObject(Settings.Instance, Formatting.Indented));
            await Console.Out.WriteLineAsync($"Writing {compositionName}...");
            await carrierFrequencyApplier.Write($"{compositionName}.wav");
            if (Settings.Instance.ConvertToMp3 && hasLame)
            {
                if (ConvertToMp3($"{compositionName}.wav"))
                {
                    await Console.Out.WriteLineAsync($"Converting {compositionName} to .mp3...");
                    await Console.Out.WriteLineAsync($"Converted {compositionName} to .mp3 using lame. Removing wav.");
                    File.Delete($"{compositionName}.wav");
                }
                else
                {
                    await Console.Out.WriteLineAsync($"    (could not convert {compositionName} to .mp3)");
                }
            }
        }

        private static bool TestForLame()
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("lame", "--version")
            {
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            try
            {
                using (var process = new Process { StartInfo = processStartInfo })
                {
                    process.Start();
                    process.WaitForExit();
                    return process.ExitCode == 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool ConvertToMp3(string fileName)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("lame", $"-V0 \"{fileName}\"")
            {
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            try
            {
                using (var lameProcess = new Process { StartInfo = processStartInfo })
                {
                    lameProcess.Start();
                    lameProcess.WaitForExit();
                    return lameProcess.ExitCode == 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static readonly Random random = new Random();
        private static readonly ConcurrentDictionary<string, string[]> nameListCache = new ConcurrentDictionary<string, string[]>();
        public static string GetRandomName()
        {
            var possibleNameListFiles = new[] { "female-first-names.txt", "male-first-names.txt" };
            var nameListFilesToUse = possibleNameListFiles.Where((l, i) => ((i + 1) & (int)Settings.Instance.Naming) != 0).ToArray();
            var nameListFile = nameListFilesToUse[random.Next(0, nameListFilesToUse.Length)];
            var nameList = nameListCache.GetOrAdd(nameListFile, s => File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, s)).Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToArray());
            string randomName = nameList[random.Next(0, nameList.Length)];
            var randomNameCased = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(randomName.ToLower());
            return randomNameCased;
        }
    }
}
