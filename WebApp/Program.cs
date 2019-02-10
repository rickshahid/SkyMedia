using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

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
            webHost.Run();
        }
    }
}