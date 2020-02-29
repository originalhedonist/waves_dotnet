using Lamar;
using Microsoft.Extensions.DependencyInjection;
using System;
using wavegenerator.library;
using wavegenerator.models;

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
                r.AddTransient<IParameterizedResolver, ParameterizedResolver>();
                r.Injectable<ChannelSettingsModel>();
                r.AddTransient<PulseFrequencyModel>(provider => provider.GetRequiredService<ChannelSettingsModel>().PulseFrequency);
                additionalRegistrations?.Invoke(r);
            });
            return container;
        }
    }
}
