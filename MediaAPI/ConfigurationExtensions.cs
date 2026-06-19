using Application.Services.Implementations;
using Application.Services.Interfaces;
using Domain.Stores;
using Domain.Stores.MongoDB;
using Infrastructure.Database;
using Infrastructure.Storage;
using Minio.AspNetCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Npgsql;
using Persistence.Repositories;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;

namespace MediaAPI
{
    public static class ConfigurationExtensions
    {
        public static void ConfigureInfrastructure(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton(sp =>
            {
                var cs = builder.Configuration
                    .GetConnectionString("DefaultConnection");

                var dataSourceBuilder =
                    new NpgsqlDataSourceBuilder(cs);

                dataSourceBuilder.ConfigureMessengerPostgresTypes();

                return dataSourceBuilder.Build();
            });

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            builder.Services.AddSingleton<IMongoClient>(_ =>
            {
                var connection =
                    builder.Configuration["Mongo:ConnectionString"]
                    ?? throw new ArgumentNullException("Mongo connection");

                return new MongoClient(connection);
            });

            builder.Services.AddSingleton<UpdatesContext>();
            builder.Services.AddSingleton<IDbConnectionFactory, NpgSqlConnectionFactory>();

            builder.Services.AddMinio(options =>
            {
                options.AccessKey = builder.Configuration.GetSection("MinioOptions").GetValue<string>("AccessKey") ?? throw new ArgumentNullException("MinIo Access Key");
                options.SecretKey = builder.Configuration.GetSection("MinioOptions").GetValue<string>("SecretKey") ?? throw new ArgumentNullException("MinIo Secret Key");
                options.Endpoint = builder.Configuration.GetSection("MinioOptions").GetValue<string>("Endpoint") ?? throw new ArgumentNullException("MinIo Endpoint");
            });
            builder.Services.Configure<Infrastructure.Models.MinioOptions>(builder.Configuration.GetSection("MinioOptions"));
            builder.Services.AddSingleton<IObjectStorage, MinioStorage>();
        }

        public static void ConfigureLogging(this WebApplicationBuilder builder)
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
        }

        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<INewMediaStore, NewMediaRepository>();
            builder.Services.AddSingleton<IMediaStore, MediaRepository>();
            builder.Services.AddScoped<IMediaService, MediaService>();
        }
    }
}
