using Infrastructure.Database;
using Infrastructure.Storage;
using Minio;
using Minio.AspNetCore;
using Npgsql;

namespace UserAPI.Extensions
{
    public static class InfrastructureConfigurationExtensions
    {
        public static void ConfigureDatabaseConnectionFactory(this WebApplicationBuilder builder)
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
            builder.Services.Configure<Infrastructure.Models.MinioOptions>(builder.Configuration.GetSection("MinioOptions"));
            builder.Services.AddSingleton<IObjectStorage, MinioStorage>();
        }
    }
}
