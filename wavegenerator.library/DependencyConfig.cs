using System;
using Ultimate.DI;
using wavegenerator.models;

namespace wavegenerator.library
{
    public class DependencyConfig
    {
        public static IContainer ConfigureContainer(Settings settings, Action<IContainer> additionalRegistrations = null) => ConfigureContainer(a =>
        {
            a.AddInstance(settings);
            a.AddInstance<IWaveFileMetadata>(settings);
            additionalRegistrations?.Invoke(a);
        });

        public static IContainer ConfigureContainer(Action<IContainer> additionalRegistrations = null)
        {
            var container = new Container();
            container.AddTransient<IPerChannelComponent, RiseApplier>();
            container.AddTransient<IPerChannelComponent, BreakApplier>();
            container.AddTransient<IPerChannelComponent, WetnessApplier>(); // which in turn uses pulse generator
            container.AddTransient<IPerChannelComponent, CarrierFrequencyApplier>();
            container.AddTransient<PulseGenerator>();
            container.AddTransient<IParameterizedResolver, ParameterizedResolver>();
            container.AddSingleton<IFeatureChooser, FeatureChooser>();
            container.AddTransient<ISectionsProvider, SectionsProvider>();
            container.AddTransient<ChannelSplitter>();
            container.AddTransient<ChannelComponentStack>();
            container.AddTransient<Randomizer>();
            container.AddTransient<FeatureProvider>();
            container.AddInstance<ISamplingFrequencyProvider>(new SamplingFrequencyProvider(44100));
            container.AddTransient<WaveStream>();
            additionalRegistrations?.Invoke(container);
            return container;
        }
    }
}
