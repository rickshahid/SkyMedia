using System.IO;
using System.Diagnostics;

using Microsoft.AspNetCore.Hosting;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string rootDirectory = Directory.GetCurrentDirectory();
            WebHostBuilder webHostBuilder = new WebHostBuilder();
            webHostBuilder.UseKestrel();
            webHostBuilder.UseContentRoot(rootDirectory);
            webHostBuilder.UseIISIntegration();
            webHostBuilder.UseStartup<Startup>();
            webHostBuilder.UseApplicationInsights();
            IWebHost webHost = webHostBuilder.Build();

            if (Debugger.IsAttached)
            {
                rootDirectory = string.Concat(rootDirectory, @"\Models");
                DocumentClient documentClient = new DocumentClient();
                documentClient.Initialize(rootDirectory);
            }

            webHost.Run();
        }
    }
}