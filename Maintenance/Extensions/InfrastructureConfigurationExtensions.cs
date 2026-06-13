using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;
using Microsoft.Extensions.Logging;

namespace Maintenance.Extensions
{
    public static class InfrastructureConfigurationExtensions
    {
        public static void ConfigureLogging(this HostApplicationBuilder builder)
        {
            ConfigurationManager configuration = builder.Configuration;
            LokiCredentials lokiCredentials = new LokiCredentials()
            {
                Login = builder.Configuration.GetSection("LokiOptions").GetValue<string>("User") ?? throw new ArgumentNullException("Loki User"),
                Password = builder.Configuration.GetSection("LokiOptions").GetValue<string>("Password") ?? throw new ArgumentNullException("Loki Password")
            };

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.GrafanaLoki(
                    builder.Configuration["LokiOptions:URI"] ?? throw new ArgumentNullException("Loki URI"),
                    credentials: lokiCredentials,
                    labels: new List<LokiLabel>()
                    {
                        new()
                        {
                            Key = "app",
                            Value = "Maintenance"
                        }
                    },
                    propertiesAsLabels: new[]
                    {
                        "level"
                    },
                    restrictedToMinimumLevel: LogEventLevel.Information
                ).CreateLogger();

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(Log.Logger);
        }
    }
}
