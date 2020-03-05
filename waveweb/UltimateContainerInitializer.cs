using Ultimate.DI;
using wavegenerator.library;

namespace waveweb
{
    public class UltimateContainerInitializer
    {
        public static IContainer Initialize()
        {
            var container = new Container();
            container.AddTransient<PulseGenerator>();
            container.AddTransient<Randomizer>();
            container.AddTransient<FeatureProvider>();
            container.AddTransient<ISectionsProvider, SectionsProvider>();
            return container;
        }
    }
}
