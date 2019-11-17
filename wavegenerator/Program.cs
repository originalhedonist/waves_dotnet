using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace wavegenerator
{
    public class Program
    {
        private static bool hasLame;

        public static async Task Main(string[] args)
        {
            var constantProviderOverrides = args.Where(a => File.Exists(a)).Select(a => new FileConstantProvider(a)).ToArray();
            ConstantsParameterization.ParameterizeConstants(constantProviderOverrides);
            hasLame = Constants.ConvertToMp3 && TestForLame();
            var tasks = Enumerable.Range(0, Constants.NumFiles)
                .Select(i => WriteFile(i))
                .ToArray();
            await Task.WhenAll(tasks);

            ConsoleWriter.WriteLine($"{tasks.Length} file(s) successfully created.", ConsoleColor.Green);
        }

        private static async Task WriteFile(int uniqueifier)
        {
            var compositionName = $"{GetRandomName()}_{DateTime.Now.ToString("yyyyMMdd_HHmm")}_{uniqueifier}";
            var pulseGenerator = new PulseGenerator(
                compositionName,
                sectionLengthSeconds: Constants.SectionLength,
                numSections: Constants.NumSections,
                channels: 2);
            var carrierFrequencyApplier = new CarrierFrequencyApplier(pulseGenerator,
                Constants.CarrierFrequencyLeftStart,
                Constants.CarrierFrequencyLeftEnd,
                Constants.CarrierFrequencyRightStart,
                Constants.CarrierFrequencyRightEnd);

            var constantsStrings = typeof(Constants).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Select(f => $"{f.Name} = {f.GetValue(null)}").ToArray();
            await File.WriteAllLinesAsync($"{compositionName}.parameters.txt", constantsStrings);
            await Console.Out.WriteLineAsync($"Writing {compositionName}...");
            await carrierFrequencyApplier.Write($"{compositionName}.wav");
            if (Constants.ConvertToMp3 && hasLame)
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
            var nameListFilesToUse = possibleNameListFiles.Where((l, i) => ((i + 1) & Constants.Naming) != 0).ToArray();
            var nameListFile = nameListFilesToUse[random.Next(0, nameListFilesToUse.Length)];
            var nameList = nameListCache.GetOrAdd(nameListFile, s => File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, s)).Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToArray());
            string randomName = nameList[random.Next(0, nameList.Length)];
            var randomNameCased = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(randomName.ToLower());
            return randomNameCased;
        }
    }

    public class TabletopTest : TabletopGenerator
    {
        public TabletopTest(double baseFrequency, int sectionLengthSeconds, int numSections, short channels) : base(baseFrequency, sectionLengthSeconds, numSections, channels)
        {
        }

        protected override TabletopParams CreateTabletopParamsForSection(int section)
        {
            return new TabletopParams
            {
                RampLength = 1,
                TopLength = 4,
                RampsUseSin2 = true
            };
        }

        protected override double CreateTopFrequency(int section)
        {
            return 261.6 * 2;
        }
    }

}
