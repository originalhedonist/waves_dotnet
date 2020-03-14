using System;
using Ultimate.DI;
using wavegenerator.models;

namespace wavegenerator.library
{
    public class DependencyConfig
    {
        public static IContainer ConfigureContainer(SettingsCommon settings, Action<IContainer> additionalRegistrations = null) => ConfigureContainer(a =>
        {
            if(settings == null) throw new ArgumentNullException(nameof(settings), "Settings must be non-null");

            if (settings is Settings settingsV1)
            {
                a.AddInstance(settingsV1);
                a.AddTransient<IWaveStream, WaveStreamV1>();
                a.AddInstance<IWaveFileMetadata>(settingsV1);
            }
            else if (settings is SettingsV2 settingsV2)
            {
                a.AddInstance(settingsV2);
                a.AddTransient<IWaveStream, WaveStreamV2>();
            }
            else
            {
                throw new ArgumentException($"Settings must be a specific version. {settings.GetType().Name} not recognized.");
            }

            additionalRegistrations?.Invoke(a);
        });

        private static IContainer ConfigureContainer(Action<IContainer> additionalRegistrations = null)
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
            container.AddTransient<Mp3Stream>();
            additionalRegistrations?.Invoke(container);
            return container;
        }
    }
}
