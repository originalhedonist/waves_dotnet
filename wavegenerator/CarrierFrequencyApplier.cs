using System;

namespace wavegenerator
{
    public class CarrierFrequencyApplier : FrequencyFunctionWaveFile
    {
        private readonly WaveFile[] patterns;

        public CarrierFrequencyApplier(WaveFile[] patterns) : 
            base(lengthSeconds: Settings.Instance.TrackLength.TotalSeconds,
                channels: Settings.Instance.NumberOfChannels,
                phaseShiftChannels: Settings.Instance.PhaseShiftCarrier)
        {
            this.patterns = patterns;
        }

        public override double Amplitude(double t, int n, int channel)
        {
            double carrierAmplitude = base.Amplitude(t, n, channel);
            double patternAmplitude = Math.Abs(patterns.ForChannel(channel).Amplitude(t, n, channel));
            return carrierAmplitude * patternAmplitude;
        }

        protected override double Frequency(double t, int n, int channel)
        {
            ChannelSettingsModel channelSettingsModel = Settings.Instance.ChannelSettings.ForChannel(channel);
            return channel == 0 ?
                channelSettingsModel.CarrierFrequency.Left :
                channelSettingsModel.CarrierFrequency.Right;
        }
    }
}
