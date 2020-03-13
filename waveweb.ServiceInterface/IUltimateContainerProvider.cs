using Ultimate.DI;
using wavegenerator.models;

namespace waveweb.ServiceInterface
{
    public interface IUltimateContainerProvider
    {
        IContainer GetFullFeatureContainer(SettingsCommon settings);
        IContainer GetFastContainer();
    }
}