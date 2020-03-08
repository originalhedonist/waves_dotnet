using Microsoft.Extensions.Configuration;
using Ultimate.DI;
using wavegenerator.library;
using waveweb.ServerComponents;
using waveweb.ServiceInterface;

namespace waveweb
{
    public class UltimateContainerProvider : IUltimateContainerProvider
    {
        private readonly IConfiguration configuration;

        public UltimateContainerProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IContainer GetFastContainer()
        {
            var container = new Container();
            container.AddTransient<PulseGenerator>();
            container.AddTransient<Randomizer>();
            container.AddTransient<FeatureProvider>();
            container.AddTransient<ISectionsProvider, SectionsProvider>();
            return container;
        }

        public IContainer GetFullFeatureContainer() => DependencyConfig.ConfigureContainer(a =>
        {
            a.AddTransient<IProgressReporter, WebProgressReporter>();
            a.AddTransient<IJobProgressProvider, JobProgressProvider>();
            a.AddInstance<IConfiguration>(configuration);
        });
    }
}
