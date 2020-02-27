using Autofac;

namespace wavegenerator
{
    public class SettingsModule : Module
    {
        private readonly Settings settings;

        public SettingsModule(Settings settings)
        {
            this.settings = settings;
        }
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(settings).As<Settings>();
        }
    }
}
