using Lamar;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace wavegenerator
{
    public class DependencyConfig
    {
        public static IContainer ConfigureContainer(Action<IServiceCollection> additionalRegistrations = null )
        {
            var container = new Container(r =>
            {
                r.AddTransient<IPerChannelComponent, RiseApplier>();
                r.AddTransient<IPerChannelComponent, BreakApplier>();
                r.AddTransient<IPerChannelComponent, PulseGenerator>();
                r.AddTransient<IPerChannelComponent, CarrierFrequencyApplier>();
                r.Injectable<ChannelSettingsModel>();
                additionalRegistrations?.Invoke(r);
            });
            return container;
        }
    }
}
