using Maintenance.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace Maintenance
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.json");
            builder.Configuration.AddJsonFile("infrastructureoptions.json");

            builder.ConfigureInfrastructure();
            builder.ConfigureLogging();

            builder.ConfigureServices();

            builder.ConfigureJobs();
            builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = false);

            IHost host = builder.Build();
            await host.RunAsync();
        }
    }
}
