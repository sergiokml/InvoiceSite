using Cve.CenLib.Core;
using Cve.GraphLib.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
namespace PortalFacturas;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(
                webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }
            )
            .GraphLibBuild()
            .CenBuild();
    }
}
