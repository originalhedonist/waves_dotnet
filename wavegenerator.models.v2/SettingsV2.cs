using System.Collections.Generic;

namespace wavegenerator.models
{
    public class SettingsV2 : SettingsCommon
    {
        public PulseComponent Phase { get; set; }
        public Dictionary<string, PulseSettingsV2> Channels { get; set; } // could be an array (keys don't matter), just makes the json easier to understand
    }

    public class PulseSettingsV2
    {
        public PulseComponent[] Components { get; set; }

        //also breaks, wetness, rises, etc.
    }

    public class PulseComponent
    {
        public string Frequency { get; set; }
        public string Pulse { get; set; }
    }
}
