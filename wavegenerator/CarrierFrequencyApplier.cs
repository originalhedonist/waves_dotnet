using System;
using System.Threading.Tasks;

namespace wavegenerator
{
    public class CarrierFrequencyApplier : FrequencyFunctionWaveFile
    {
        private readonly WaveFile[] patterns;

        public CarrierFrequencyApplier(WaveFile[] patterns) : 
            base(phaseShiftChannels: Settings.Instance.PhaseShiftCarrier)
        {
            this.patterns = patterns;
        }

        public override async Task<double> Amplitude(double t, int n, int channel)
        {
            double carrierAmplitude = await base.Amplitude(t, n, channel);
            var pattern = patterns.ForChannel(channel);
            double patternAmplitude = Math.Abs(await pattern.Amplitude(t, n, channel));
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
