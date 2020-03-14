namespace wavegenerator.models
{
    public class SettingsV2 : SettingsCommon
    {
        public PulseSettingsV2 Left { get; set; }
        public PulseSettingsV2 Right { get; set; }
    }

    public class PulseSettingsV2
    {
        public string Frequency { get; set; }
        public string Pulse { get; set; }
    }
}
