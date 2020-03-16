using System.Collections.Generic;

namespace wavegenerator.models
{
    public class SettingsV2 : SettingsCommon
    {
        public BreaksModel Breaks { get; set; }
        public RisesModel Rises { get; set; }
        public FrequencyPulse Phase { get; set; }
        public Dictionary<string, ChannelSettingsV2> Channels { get; set; } // could be an array (keys don't matter), just makes the json easier to understand

        public override short GetNumberOfChannels() => (short)Channels.Count;
    }

    public class ChannelSettingsV2
    {
        public string Wetness { get; set; }
        public FrequencyPulse Carrier { get; set; }
        public FrequencyPulse[] Components { get; set; }
        //also breaks, rises, etc.
    }

    public class FrequencyPulse
    {
        public string Frequency { get; set; }
        public string Pulse { get; set; }
    }
}
