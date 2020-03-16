using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using ServiceStack;

namespace waveweb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseModularStartup<Startup>()
                .Build();
    }
}
