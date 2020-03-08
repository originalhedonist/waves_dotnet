using Ultimate.DI;

namespace waveweb.ServiceInterface
{
    public interface IUltimateContainerProvider
    {
        IContainer GetFullFeatureContainer();
        IContainer GetFastContainer();
    }
}