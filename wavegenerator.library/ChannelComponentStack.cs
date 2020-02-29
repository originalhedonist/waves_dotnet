using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wavegenerator.library
{
    public class ChannelComponentStack : IAmplitude
    {
        private readonly IPerChannelComponent[] components;

        public ChannelComponentStack(IEnumerable<IPerChannelComponent> components)
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
