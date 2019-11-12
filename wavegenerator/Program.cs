using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace wavegenerator
{
    public class Program
    {
        public static async Task Main()
        {
            ConstantsParameterization.ParameterizeConstants();
            var tasks = Enumerable.Range(0, Constants.NumFiles)
                .Select(i => Task.Run(() => WriteFile(i)))
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
                carrierFrequencyRight: 600,
                carrierFrequencyLeft: 600);
            var constantsStrings = typeof(Constants).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Select(f => $"{f.Name} = {f.GetValue(null)}").ToArray();
            await File.WriteAllLinesAsync($"{compositionName}.parameters.txt", constantsStrings);
            await Console.Out.WriteLineAsync($"Writing {compositionName}...");
            await carrierFrequencyApplier.Write($"{compositionName}.wav");
        }

        private static readonly Random random = new Random();
        private static readonly ConcurrentDictionary<string, string[]> nameListCache = new ConcurrentDictionary<string, string[]>();
        public static string GetRandomName()
        {
            var possibleNameListFiles = new[] { "female-first-names.txt", "male-first-names.txt" };
            var nameListFilesToUse = possibleNameListFiles.Where((l, i) => (i & Constants.Naming) != 0).ToArray();
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


    public class Jump : FrequencyFunctionWaveFile
    {
        public Jump(int lengthSeconds, short channels) : base(lengthSeconds, channels)
        {
        }

        protected override double Frequency(double t, int n, int channel)
        {
            if (t < 1)
                return 261.6;
            else
                return 261.1 * 2;
        }
    }

    public class Rising : FrequencyFunctionWaveFile
    {
        public Rising(int lengthSeconds, short channels) : base(lengthSeconds, channels)
        {
        }

        protected override double Frequency(double t, int n, int channel)
        {
            return 261.6 * (1 + (float)n / N);
        }
    }
}
