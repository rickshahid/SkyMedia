using System.IO;

using Microsoft.AspNetCore.Hosting;

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
            webHost.Run();
        }
    }
}