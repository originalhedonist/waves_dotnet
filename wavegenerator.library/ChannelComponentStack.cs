using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wavegenerator.library
{
    // the overall composition
    // for now, the composition is governed by simply multiplying up all the components
    // that transcend wetness.
    // one of which is the wetness applier itself
    // if the distinction becomes any more complicated than a decision between
    // "transcends wetness" vs "doesn't transcend wetness"
    // then a separate composition class will be necessary. but for now that's not necessary.
    public class ChannelComponentStack : IAmplitude
    {
        private readonly IPerChannelComponentTranscendsWetness[] components;

        public ChannelComponentStack(IEnumerable<IPerChannelComponentTranscendsWetness> components)
        {
            this.components = components.ToArray();
        }

        public async Task<double> Amplitude(double t, int n, int channel)
        {
            double retval = 1;
            foreach(var component in components)
            {
                var componentAmplitude = await component.Amplitude(t, n, channel);
                retval *= componentAmplitude;
            }
            return retval;
        }
    }
}
