using System.IO;

using Microsoft.AspNetCore.Hosting;

namespace AzureSkyMedia.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebHostBuilder hostBuilder = new WebHostBuilder();
            hostBuilder.UseKestrel();
            hostBuilder.UseContentRoot(Directory.GetCurrentDirectory());
            hostBuilder.UseIISIntegration();
            hostBuilder.UseStartup<Startup>();
            IWebHost webHost = hostBuilder.Build();
            webHost.Run();
        }
    }
}
