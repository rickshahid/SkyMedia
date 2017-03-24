using System.IO;

using Microsoft.AspNetCore.Hosting;

namespace AzureSkyMedia.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string contentRoot = Directory.GetCurrentDirectory();
            WebHostBuilder webHostBuilder = new WebHostBuilder();
            webHostBuilder.UseKestrel();
            webHostBuilder.UseContentRoot(contentRoot);
            webHostBuilder.UseIISIntegration();
            webHostBuilder.UseStartup<Startup>();
            webHostBuilder.UseApplicationInsights();
            IWebHost webHost = webHostBuilder.Build();
            webHost.Run();
        }
    }
}
