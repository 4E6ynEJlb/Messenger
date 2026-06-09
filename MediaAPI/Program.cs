using Infrastructure.Database;
using Infrastructure.Storage;
using MediaAPI.Middleware;
using Minio;
using Minio.AspNetCore;
using Npgsql;
using Prometheus;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;
using Serilog.Sinks.GrafanaLoki;

namespace MaintenanceAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Configuration.AddJsonFile("infrastructureoptions.json");
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddServer(new Microsoft.OpenApi.Models.OpenApiServer
                {
                    Url = "/media"
                });
            });
            builder.Services.AddSingleton(sp =>
            {
                var cs = builder.Configuration
                    .GetConnectionString("DefaultConnection");

                var dataSourceBuilder =
                    new NpgsqlDataSourceBuilder(cs);

                dataSourceBuilder.ConfigureMessengerPostgresTypes();

                return dataSourceBuilder.Build();
            });

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
                            Value = "MediaAPI"
                        }
                    },
                    propertiesAsLabels: new[]
                    {
                        "level"
                    },
                    restrictedToMinimumLevel: LogEventLevel.Information
                ).CreateLogger();

            builder.Host.UseSerilog();

            builder.Services.AddSingleton<IDbConnectionFactory, NpgSqlConnectionFactory>();

            builder.Services.AddMinio(options =>
            {
                options.AccessKey = builder.Configuration.GetSection("MinioOptions").GetValue<string>("AccessKey") ?? throw new ArgumentNullException("MinIo Access Key");
                options.SecretKey = builder.Configuration.GetSection("MinioOptions").GetValue<string>("SecretKey") ?? throw new ArgumentNullException("MinIo Secret Key");
                options.Endpoint = builder.Configuration.GetSection("MinioOptions").GetValue<string>("Endpoint") ?? throw new ArgumentNullException("MinIo Endpoint");
            });
            builder.Services.Configure<Infrastructure.Models.MinioOptions>(builder.Configuration.GetSection("MinioOptions"));
            builder.Services.AddSingleton<IObjectStorage, MinioStorage>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(c=>c.RouteTemplate = "swagger/{documentName}/swagger.json");
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/media/swagger/v1/swagger.json", "Media API V1");
                    c.RoutePrefix = "swagger";
                });
            }


            app.UseAuthorization();

            app.UseMiddleware<TelemetryMiddleware>();
            app.UseMiddleware<LoggingMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>();


            app.MapMetrics();
            app.MapControllers();

            app.Run();
        }
    }
}
