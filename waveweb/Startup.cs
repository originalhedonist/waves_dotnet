using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Funq;
using ServiceStack;
using waveweb.ServiceInterface;
using Hangfire;
using Microsoft.Extensions.Configuration;
using waveweb.ServerComponents;
using Microsoft.Extensions.Logging;
using wavegenerator.library;

namespace waveweb
{

    public class Startup : ModularStartup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public new void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMapper>(new Mapper(Mapping.CreateMapperConfiguration()));
            services.AddTransient<IWaveformTestPulseGeneratorProvider, WaveformTestPulseGeneratorProvider>();
            services.AddTransient<IJobScheduler, JobScheduler>();
            services.AddTransient<IJobProgressProvider, JobProgressProvider>();
            services.AddTransient<TestLongJob>();
            services.AddTransient<FileCreator>();
            services.AddTransient<IUltimateContainerProvider, UltimateContainerProvider>();
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));
            services.AddHangfireServer();

            services.AddLogging(c => 
            {
                c.AddConsole();
                c.AddAzureWebAppDiagnostics();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                logger.LogInformation("In development environment");
                app.UseDeveloperExceptionPage();
            }

            app.UseServiceStack(new AppHost
            {
                AppSettings = new NetCoreAppSettings(Configuration)
            });
            
           
        }
    }

    public class AppHost : AppHostBase
    {
        public AppHost() : base("waveweb", typeof(ServiceInterfaceAssemblyMarker).Assembly) { }

        // Configure your AppHost with the necessary configuration and dependencies your App needs
        public override void Configure(Container container)
        {
            Plugins.Add(new SharpPagesFeature()); // enable server-side rendering, see: https://sharpscript.net/docs/sharp-pages

            SetConfig(new HostConfig
            {
                UseSameSiteCookies = true,
                AddRedirectParamsToQueryString = true,
                DebugMode = AppSettings.Get(nameof(HostConfig.DebugMode), HostingEnvironment.IsDevelopment()),
            });
        }
    }
}
