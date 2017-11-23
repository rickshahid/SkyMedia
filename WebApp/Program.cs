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
            string appDirectory = Directory.GetCurrentDirectory();
            WebHostBuilder webHostBuilder = new WebHostBuilder();
            webHostBuilder.UseKestrel();
            webHostBuilder.UseContentRoot(appDirectory);
            webHostBuilder.UseIISIntegration();
            webHostBuilder.UseStartup<Startup>();
            webHostBuilder.UseApplicationInsights();
            IWebHost webHost = webHostBuilder.Build();

            if (Debugger.IsAttached)
            {
                string modelsDirectory = string.Concat(appDirectory, @"\Models");
                DocumentClient documentClient = new DocumentClient();
                documentClient.Initialize(modelsDirectory);
            }

            webHost.Run();
        }
    }
}