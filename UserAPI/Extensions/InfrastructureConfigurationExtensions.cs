using Application.Models.Internal.Constants;
using Application.Models.Internal.Messages;
using Application.Services.Consumers;
using Infrastructure.Database;
using Infrastructure.Models;
using Infrastructure.Storage;
using MassTransit;
using Minio;
using Minio.AspNetCore;
using Npgsql;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.GrafanaLoki;
using StackExchange.Redis;

namespace UserAPI.Extensions
{
    public static class InfrastructureConfigurationExtensions
    {
        public static void ConfigureDatabaseConnectionFactory(this WebApplicationBuilder builder)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            builder.Services.AddSingleton(sp =>
            {
                var cs = builder.Configuration
                    .GetConnectionString("DefaultConnection");

                var dataSourceBuilder =
                    new NpgsqlDataSourceBuilder(cs);

                dataSourceBuilder.ConfigureMessengerPostgresTypes();

                return dataSourceBuilder.Build();
            });

            builder.Services.AddSingleton<IDbConnectionFactory, NpgSqlConnectionFactory>();
        }

        public static void ConfigureMinioStorage(this WebApplicationBuilder builder)
        {
            builder.Services.AddMinio(options =>
            {
                options.AccessKey = builder.Configuration.GetSection("MinioOptions").GetValue<string>("AccessKey") ?? throw new ArgumentNullException("MinIo Access Key");
                options.SecretKey = builder.Configuration.GetSection("MinioOptions").GetValue<string>("SecretKey") ?? throw new ArgumentNullException("MinIo Secret Key");
                options.Endpoint = builder.Configuration.GetSection("MinioOptions").GetValue<string>("Endpoint") ?? throw new ArgumentNullException("MinIo Endpoint");
            });
            builder.Services.Configure<Infrastructure.Models.MinioOptions>(builder.Configuration.GetSection(Infrastructure.Models.MinioOptions.OPTIONS_NAME));
            builder.Services.AddSingleton<IObjectStorage, MinioStorage>();
        }

        public static void ConfigureRedis(this WebApplicationBuilder builder)
        {
            builder.Services.AddKeyedSingleton<IConnectionMultiplexer>(
                CacheKeys.MESSAGES,
                (_, _) =>
                {
                    return ConnectionMultiplexer.Connect(builder.Configuration["Redis:Messages"]!);
                });
            builder.Services.AddKeyedSingleton<IConnectionMultiplexer>(
                CacheKeys.BOT_USER,
                (_, _) =>
                {
                    return ConnectionMultiplexer.Connect(builder.Configuration["Redis:BotsUsers"]!);
                });
        }

        public static void ConfigureLogging(this WebApplicationBuilder builder)
        {
            ConfigurationManager configuration = builder.Configuration;
            GrafanaLokiCredentials lokiCredentials = new GrafanaLokiCredentials()
            {
                User = builder.Configuration.GetSection("LokiOptions").GetValue<string>("User") ?? throw new ArgumentNullException("Loki User"),
                Password = builder.Configuration.GetSection("LokiOptions").GetValue<string>("Password") ?? throw new ArgumentNullException("Loki Password")
            };
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.GrafanaLoki(
                    builder.Configuration.GetSection("LokiOptions").GetValue<string>("URI") ?? throw new ArgumentNullException("Loki URI"),
                    lokiCredentials,
                    new Dictionary<string, string> { { "app", $"UserAPI-{Environment.GetEnvironmentVariable("GROUP_ID")}" } },
                    LogEventLevel.Information
                ).CreateLogger();
            builder.Host.UseSerilog();
            builder.Logging.AddSerilog();
        }

        public static void ConfigureBus(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<FaultConsumeObserver>();
            
            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<BotButtonsUpdatedConsumer>();
                x.AddConsumer<ChatDeletedConsumer>();
                x.AddConsumer<FileDeletedConsumer>();
                x.AddConsumer<MessageDeletedConsumer>();
                x.AddConsumer<MessagesSentConsumer>();
                x.AddConsumer<MessageUpdatedConsumer>();
                x.AddConsumer<UserIsTypingConsumer>();

                x.UsingInMemory((context, config) =>
                {
                    config.ConnectConsumeObserver(context.GetRequiredService<FaultConsumeObserver>());
                });

                x.AddRider(rider =>
                {
                    rider.AddConsumer<BotButtonsUpdatedConsumer>();
                    rider.AddConsumer<ChatDeletedConsumer>();
                    rider.AddConsumer<FileDeletedConsumer>();
                    rider.AddConsumer<MessageDeletedConsumer>();
                    rider.AddConsumer<MessagesSentConsumer>();
                    rider.AddConsumer<MessageUpdatedConsumer>();
                    rider.AddConsumer<UserIsTypingConsumer>();

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host(builder.Configuration["Kafka:Host"]);

                        string instanceId = Environment.GetEnvironmentVariable("GROUP_ID")
                                 ?? throw new ArgumentException("GROUP_ID environment variable is not set");

                        k.TopicEndpoint<BotButtonsUpdatedMessage>(
                            "bot-buttons-updated-topic",
                            $"bot-buttons-updated-group-{instanceId}",
                            e =>
                            {
                                e.CreateIfMissing(t =>
                                {
                                    t.NumPartitions = 3;
                                    t.ReplicationFactor = 1;
                                });
                                e.ConfigureConsumer<BotButtonsUpdatedConsumer>(context);
                            });

                        k.TopicEndpoint<ChatDeletedMessage>(
                            "chat-deleted-topic",
                            $"chat-deleted-group-{instanceId}",
                            e =>
                            {
                                e.CreateIfMissing(t =>
                                {
                                    t.NumPartitions = 3;
                                    t.ReplicationFactor = 1;
                                });
                                e.ConfigureConsumer<ChatDeletedConsumer>(context);
                            });

                        k.TopicEndpoint<FileDeletedMessage>(
                            "file-deleted-topic",
                            $"file-deleted-group-{instanceId}",
                            e =>
                            {
                                e.CreateIfMissing(t =>
                                {
                                    t.NumPartitions = 3;
                                    t.ReplicationFactor = 1;
                                });
                                e.ConfigureConsumer<FileDeletedConsumer>(context);
                            });

                        k.TopicEndpoint<MessageDeletedMessage>(
                            "message-deleted-topic",
                            $"message-deleted-group-{instanceId}",
                            e =>
                            {
                                e.CreateIfMissing(t =>
                                {
                                    t.NumPartitions = 3;
                                    t.ReplicationFactor = 1;
                                });
                                e.ConfigureConsumer<MessageDeletedConsumer>(context);
                            });

                        k.TopicEndpoint<MessagesSentMessage>(
                            "messages-sent-topic",
                            $"messages-sent-group-{instanceId}",
                            e =>
                            {
                                e.CreateIfMissing(t =>
                                {
                                    t.NumPartitions = 3;
                                    t.ReplicationFactor = 1;
                                });
                                e.ConfigureConsumer<MessagesSentConsumer>(context);
                            });

                        k.TopicEndpoint<MessageUpdatedMessage>(
                            "message-updated-topic",
                            $"message-updated-group-{instanceId}",
                            e =>
                            {
                                e.CreateIfMissing(t =>
                                {
                                    t.NumPartitions = 3;
                                    t.ReplicationFactor = 1;
                                });
                                e.ConfigureConsumer<MessageUpdatedConsumer>(context);
                            });

                        k.TopicEndpoint<UserIsTypingMessage>(
                            "user-is-typing-topic",
                            $"user-is-typing-group-{instanceId}",
                            e =>
                            {
                                e.CreateIfMissing(t =>
                                {
                                    t.NumPartitions = 3;
                                    t.ReplicationFactor = 1;
                                });
                                e.ConfigureConsumer<UserIsTypingConsumer>(context);
                            });

                    });
                });
            });
        }
    }
}
