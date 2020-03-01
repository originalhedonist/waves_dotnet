using System;
using Ultimate.DI;
using wavegenerator.library;
using wavegenerator.models;

namespace wavegenerator
{
    public class DependencyConfig
    {
        public static IContainer ConfigureContainer(Action<IContainer> additionalRegistrations = null )
        {
            var container = new Container();
            container.AddTransient<IPerChannelComponentTranscendsWetness, RiseApplier>();
            container.AddTransient<IPerChannelComponentTranscendsWetness, BreakApplier>();
            container.AddTransient<IPerChannelComponentTranscendsWetness, WetnessApplier>();
            container.AddTransient<IPerChannelComponentTranscendsWetness, CarrierFrequencyApplier>();
            container.AddTransient<IPerChannelComponent, PulseGenerator>();
            container.AddTransient<IParameterizedResolver, ParameterizedResolver>();
            container.AddTransient<ISettingsSectionProvider<SectionsModel>, SectionModelProvider>();
            container.AddTransient<ISettingsSectionProvider<WetnessModel>, WetnessModelProvider>();
            container.AddTransient<ISettingsSectionProvider<PulseFrequencyModel>, PulseFrequencyModelProvider>();
            container.AddTransient<ISettingsSectionProvider<FeatureProbabilityModel>, FeatureProbabilityModelProvider>();
            container.AddTransient<ISettingsSectionProvider<RisesModel>, RiseSectionModelProvider>();
            container.AddSingleton<FeatureChooser>();
            container.AddTransient<ISectionsProvider, SectionsProvider>();
            container.AddTransient<ChannelSplitter>();
            container.AddTransient<ChannelComponentStack>();
            container.AddTransient<Randomizer>();
            container.AddTransient<FeatureProvider>();
            container.AddTransient<Probability>();
            container.AddTransient<WaveStream>();
            additionalRegistrations?.Invoke(container);
            return container;
        }
    }
}
