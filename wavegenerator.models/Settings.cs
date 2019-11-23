using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using wavegenerator.models;

namespace wavegenerator
{
    public class Settings
    {
        public static Settings Default = new Settings();
        public static Settings Instance { get; set; }

        [Description("Whether to use randomization")]
#if DEBUG
        public bool Randomization { get; set; }
#else
        public bool Randomization {get; set;};
#endif

        public bool ConvertToMp3 { get; set; }

        public int NumFiles { get; set; }

        public NamingConvention Naming { get; set; }

        [Range(typeof(TimeSpan), "00:00:30", "13:31:35")]
#if DEBUG
        public TimeSpan TrackLength { get; set; }
#else
        public TimeSpan TrackLength {get; set;}
#endif
        [Range(1, 2)]
        public short NumberOfChannels { get; set; }

        [Description("Whether the right channel's carrier signal will be phase shifted from the left's")]
        public bool PhaseShiftCarrier { get; set; }

        [Description("Whether the pulsing of the right channel will be phase-shifted from the left")]
        public bool PhaseShiftPulses { get; set; }

        [ValidateObject]
        [ValidateChannelCount]
        [ValidateRiserLength]
        public ChannelSettingsModel[] ChannelSettings { get; set; }

        public int NumSections(int channel) => (int)(TrackLength / ChannelSettings.ForChannel(channel).Sections.TotalLength);
    }

    public static class SettingsExtensions
    {
        public static T ForChannel<T>(this T[] items, int channel) => Settings.Instance.ChannelSettings.Length > 1 ? items[channel] : items.Single();
    }
}
