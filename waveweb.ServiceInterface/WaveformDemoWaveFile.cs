﻿using System.Threading.Tasks;
using wavegenerator;

namespace waveweb.ServiceInterface
{
    public class WaveformDemoWaveFile : FrequencyFunctionWaveFile
    {
        public WaveformDemoWaveFile() : base(phaseShiftChannels: false) { }
        protected override Task<double> Frequency(double t, int n, int channel)
        {
            throw new System.NotImplementedException();
        }
    }

}