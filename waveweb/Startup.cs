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
using System.IO;
using System.Linq;
using System;
using wavegenerator.library;
using Hangfire.Common;

namespace waveweb
{

    public class Startup : ModularStartup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public new void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<Ultimate.ORM.IObjectMapper, Ultimate.ORM.ObjectMapper>();
            services.AddSingleton<AutoMapper.IMapper>(new AutoMapper.Mapper(Mapping.CreateMapperConfiguration()));
            services.AddTransient<IWaveformTestPulseGeneratorProvider, WaveformTestPulseGeneratorProvider>();
            services.AddTransient<IJobScheduler, JobScheduler>();
            services.AddTransient<IJobProgressProvider, JobProgressProvider>();
            services.AddTransient<FileCreator>();
            services.AddTransient<IUltimateContainerProvider, UltimateContainerProvider>();
            services.AddTransient<IOutputDirectoryProvider, WebOutputDirectoryProvider>();
            services.AddSingleton<RecaptchaVerifier>();

            services.AddHangfire(x =>
            {
                x.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection"));
                RecurringJob.AddOrUpdate(nameof(DeleteOldFiles), () => DeleteOldFiles(), "15 7 * * * *");
                RecurringJob.Trigger(nameof(DeleteOldFiles));
            });
            services.AddHangfireServer();

            services.AddLogging(c =>
            {
                c.AddConsole();
                c.AddAzureWebAppDiagnostics();
            });

        }

        public static void DeleteOldFiles()
        {
            // delete all files that haven't been accessed for 24 hours
            var limit = DateTime.Now.AddHours(-24);
            var directoryProvider = new WebOutputDirectoryProvider();

            foreach (var oldFile in new DirectoryInfo(directoryProvider.GetOutputDirectory()).GetFiles().Where(fi => fi.LastAccessTime < limit))
            {
                try { File.Delete(oldFile.FullName); } catch (Exception) { }
            }

            // delete all wav files that haven't been written in half an hour
            var wavWriteLimit = DateTime.Now.AddMinutes(-30);
            foreach(var oldFile in new DirectoryInfo(WebOutputDirectoryProvider.OutputDir).GetFiles("*.wav").Where(fi => fi.LastWriteTime < wavWriteLimit))
            {
                try { File.Delete(oldFile.FullName); } catch (Exception) { }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();


            if (env.IsDevelopment())
            {
                logger.LogInformation("In development environment");
                app.UseDeveloperExceptionPage();
                configBuilder.AddUserSecrets<Startup>();
            }

            Configuration = configBuilder.Build();
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
