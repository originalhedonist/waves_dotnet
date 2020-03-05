using Ultimate.DI;
using wavegenerator.library;
using waveweb.ServiceInterface;

namespace waveweb
{
    public class FullFeatureUltimateContainerProvider : IFullFeatureUltimateContainerProvider
    {
        public IContainer GetContainer() => DependencyConfig.ConfigureContainer(a =>
        {
            a.AddTransient<IProgressReporter, WebProgressReporter>();
        });
    }
}
