using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ultimate.DI;
using wavegenerator.library;
using wavegenerator.models;
using waveweb.ServerComponents;
using waveweb.ServiceInterface;

namespace waveweb
{
    public class UltimateContainerProvider : IUltimateContainerProvider
    {
        private readonly IConfiguration configuration;
        private readonly ILoggerFactory loggerProvider;

        public UltimateContainerProvider(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.configuration = configuration;
            this.loggerProvider = loggerFactory;
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

        public IContainer GetFullFeatureContainer(SettingsCommon settings) => DependencyConfig.ConfigureContainer(settings, a =>
        {
            a.AddTransient<IProgressReporter, WebProgressReporter>();
            a.AddTransient<IJobProgressProvider, JobProgressProvider>();
            a.AddTransient<Ultimate.ORM.IObjectMapper, Ultimate.ORM.ObjectMapper>();
            a.AddInstance<IConfiguration>(configuration);
            a.AddInstance<ILoggerFactory>(loggerProvider);
            a.AddTransient<IOutputDirectoryProvider, WebOutputDirectoryProvider>();
        });
    }
}
