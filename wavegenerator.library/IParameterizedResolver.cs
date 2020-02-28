using System;

namespace wavegenerator.library
{
    public interface IParameterizedResolver
    {
        T GetRequiredService<T>(Action<IInjector> injections);
    }

    public interface IInjector
    {
        void Inject<T>(T t);
    }
}
