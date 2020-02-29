using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using wavegenerator.library;

namespace wavegenerator
{

    public class Program
    {
        private static bool hasLame;
        private static bool acceptName;
        public static async Task Main(string[] allArgs)
        {

            var stopwatch = Stopwatch.StartNew();
            try
            {
                string[] options = allArgs.Where(a => a.StartsWith("--")).Select(a => a.Substring(2)).ToArray();
                string[] args = allArgs.Where(a => !a.StartsWith("--")).ToArray();
                acceptName = options.Contains("acceptName", StringComparer.CurrentCultureIgnoreCase);

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
                    var settings = SettingsLoader.LoadAndValidateSettings(filePath);
                    var container = DependencyConfig.ConfigureContainer(r => r.AddSingleton(settings));

                    string[] names = new string[settings.NumFiles];
                    for(int i = 0; i < settings.NumFiles; i++)
                    {
                        names[i] = await GetName(settings, i);
                    }

                    var maxNameLength = names.Max(n => n.Length);
                    for(int i = 0; i < settings.NumFiles; i++)
                    {
                        Console.WriteLine($"Writing {names[i].PadRight(maxNameLength)}...");
                    }

                    hasLame = settings.ConvertToMp3 && TestForLame();
                    var tasks = Enumerable.Range(0, settings.NumFiles)
                        .Select(i => WriteFile(container, settings, i, names[i]))
                        .ToArray();
                    await Task.WhenAll(tasks);

                    stopwatch.Stop();
                    ConsoleWriter.WriteLine($"{tasks.Length} file(s) successfully created in {stopwatch.Elapsed}", ConsoleColor.Green);
                }
            }
            catch (Exception ex)
            {
                ConsoleWriter.WriteLine(ex.Message, ConsoleColor.Red);
                Console.WriteLine();
                Console.WriteLine($"If the program will not accept the settings file you are using, please go to https://github.com/originalhedonist/waves_dotnet/issues and create an issue, attaching the file, and/or email originalhedonist@gmail.com, and I will convert the file into a format that can be read by the current version of the program for you. ");
            }
        }

        public static readonly object ConsoleLockObj = new object();

        private static async Task WriteFile(IContainer componentContext, Settings settings, int uniqueifier, string name)
        {
            var compositionName = $"{name}_{DateTime.Now.ToString("yyyyMMdd_HHmm")}_{uniqueifier}";
            var waveStream = componentContext.GetRequiredService<WaveStream>();
            await File.WriteAllTextAsync($"{compositionName}.parameters.json", JsonConvert.SerializeObject(settings, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore } ));

            await waveStream.Write($"{compositionName}.wav");

            if (settings.ConvertToMp3 && hasLame)
            {
                if (ConvertToMp3($"{compositionName}.wav"))
                {
                    File.Delete($"{compositionName}.wav");
                }
                else
                {
                    ConsoleWriter.WriteLine($"(could not convert {compositionName} to .mp3)", ConsoleColor.Red);
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

        public static async Task<string> GetName(Settings settings, int fileIndex)
        {
            if (settings.Naming.Specific != null)
            {
                return settings.Naming.Specific[fileIndex];
            }
            else
            {
                if (acceptName)
                {
                    await Console.Out.WriteLineAsync("Any key to re-generate, leave 10 seconds (or Y) to accept");
                    do
                    {
                        var candidate = GetRandomNameInternal(settings);
                        Console.Write(candidate);
                        if (await Accept())
                        {
                            return candidate;
                        }
                    } while (true);
                }
                else
                {
                    return GetRandomNameInternal(settings);
                }
            }
        }

        private static async Task<bool> Accept()
        {
            bool retval = true;
            for (int i = 0; i < 10; i++)
            {
                Console.Write(".");
                await Task.Delay(TimeSpan.FromSeconds(1));
                if(Console.KeyAvailable)
                {
                    retval = Console.ReadKey(true).Key == ConsoleKey.Y;
                    break;
                }
            }
            await Console.Out.WriteLineAsync();
            return retval;
        }

        public static string GetRandomNameInternal(Settings settings)
        {
            var possibleNameListFiles = new[] { "female-first-names.txt", "male-first-names.txt" };
            var nameListFilesToUse = possibleNameListFiles.Where((l, i) => ((i + 1) & (int)settings.Naming.Strategy.Value) != 0).ToArray();
            var nameListFile = nameListFilesToUse[random.Next(0, nameListFilesToUse.Length)];
            var nameList = nameListCache.GetOrAdd(nameListFile, s => File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, s)).Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToArray());
            string randomName = nameList[(int)(Math.Pow(random.NextDouble(), 1.5) * nameList.Length)];
            var randomNameCased = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(randomName.ToLower());
            return randomNameCased;
        }


    }
}
