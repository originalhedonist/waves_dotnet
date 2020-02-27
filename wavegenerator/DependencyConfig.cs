using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using System.Reflection;

namespace wavegenerator
{
    public class DependencyConfig
    {
        public static IComponentContext ConfigureContainer(params IModule[] modules)
        {
            var containerBuilder = new ContainerBuilder();
            foreach (var module in modules)
            {
                containerBuilder.RegisterModule(module);
            }
            containerBuilder.RegisterType<RiseApplier>().As<IPerChannelComponent>();
            containerBuilder.RegisterType<BreakApplier>().As<IPerChannelComponent>();
            containerBuilder.RegisterType<PulseGenerator>().As<IPerChannelComponent>();
            containerBuilder.RegisterType<CarrierFrequencyApplier>().As<IPerChannelComponent>();
            containerBuilder.RegisterType<ChannelComponentStack>().AsSelf();
            containerBuilder.RegisterType<ChannelSplitter>().AsSelf();
            containerBuilder.RegisterType<WaveStream>().AsSelf();
            containerBuilder.RegisterType<Probability>().AsSelf();
            containerBuilder.RegisterType<Randomizer>().AsSelf();
            
            return containerBuilder.Build();
        }
    }
}
