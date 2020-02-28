using Lamar;
using System;
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
            using (var nestedContainer = container.GetNestedContainer())
            {
                var injector = new Injector(nestedContainer);
                injections(injector);
                return nestedContainer.GetInstance<T>();
            }
        }
    }

    public class Injector : IInjector
    {
        private readonly INestedContainer nestedContainer;

        public Injector(INestedContainer nestedContainer)
        {
            this.nestedContainer = nestedContainer;
        }
        public void Inject<T>(T t) => nestedContainer.Inject<T>(t);
    }
}
