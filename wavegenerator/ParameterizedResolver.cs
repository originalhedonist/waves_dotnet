using System;
using Ultimate.DI;
using wavegenerator.library;

namespace wavegenerator
{
    // lamar specific
    public class ParameterizedResolver : IParameterizedResolver
    {
        private readonly IContainer container;

        public ParameterizedResolver(IContainer container)
        {
            this.container = container;
        }

        public T GetRequiredService<T>(Action<IInjector> injections)
        {
            var nestedContainer = container.GetNestedContainer();
            var injector = new Injector(nestedContainer);
            injections(injector);
            var resolution = nestedContainer.Resolve<T>();
            return resolution;
        }
    }

    public class Injector : IInjector
    {
        private readonly IContainer nestedContainer;

        public Injector(IContainer nestedContainer)
        {
            this.nestedContainer = nestedContainer;
        }
        public void Inject<T>(T t) => nestedContainer.AddInstance<T>(t);
    }
}
