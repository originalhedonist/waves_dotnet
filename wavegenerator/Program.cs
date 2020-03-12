using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Ultimate.DI;
using wavegenerator.library;
using wavegenerator.models;

namespace wavegenerator
{

    public class Program
    {
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
                    var container = DependencyConfig.ConfigureContainer(settings, c =>
                    {
                        c.AddTransient<IOutputDirectoryProvider, CurrentDirectoryProvider>();
                        c.AddInstance<IProgressReporter>(new ConsoleProgressReporter());
                    });

                    var name = await GetName(settings);
                    
                    Console.WriteLine($"Writing {name}...");

                    await WriteFile(container, settings, name);

                    stopwatch.Stop();
                    ConsoleWriter.WriteLine($"File successfully created in {stopwatch.Elapsed}", ConsoleColor.Green);
                }
            }
            catch (Exception ex)
            {
                ConsoleWriter.WriteLine(ex.Message, ConsoleColor.Red);
                Console.WriteLine();
                Console.WriteLine($"If the program will not accept the settings file you are using, please go to https://github.com/originalhedonist/waves_dotnet/issues and create an issue, attaching the file, and/or email originalhedonist@gmail.com, and I will convert the file into a format that can be read by the current version of the program for you. ");
            }
        }

        private static async Task WriteFile(IContainer componentContext, Settings settings, string name)
        {
            var compositionName = $"{name}_{DateTime.Now.ToString("yyyyMMdd_HHmm")}";
            var mp3Stream = componentContext.Resolve<Mp3Stream>();
            await File.WriteAllTextAsync($"{compositionName}.parameters.json", JsonConvert.SerializeObject(settings, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore } ));

            await mp3Stream.Write($"{compositionName}.mp3");
        }

        private static readonly Random random = new Random();
        private static readonly ConcurrentDictionary<string, string[]> nameListCache = new ConcurrentDictionary<string, string[]>();

        public static async Task<string> GetName(Settings settings)
        {
            if(settings.Naming == null)
            {
                return $"{DateTime.Now:yyyyMMdd_HHmmss}";
            }
            else if (settings.Naming.Specific != null)
            {
                return settings.Naming.Specific;
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
