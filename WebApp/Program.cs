using System.IO;
using System.Diagnostics;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IWebHostBuilder webHostBuilder = WebHost.CreateDefaultBuilder(args);
            webHostBuilder.UseStartup<Startup>();
            webHostBuilder.UseApplicationInsights();
            IWebHost webHost = webHostBuilder.Build();
            if (Debugger.IsAttached)
            {
                string appDirectory = Directory.GetCurrentDirectory();
                string modelsDirectory = Path.Combine(appDirectory, Constant.WebModels);
                using (DatabaseClient databaseClient = new DatabaseClient())
                {
                    databaseClient.Initialize(modelsDirectory);
                }
            }
            webHost.Run();
        }
    }
}