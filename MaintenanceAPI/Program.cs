
using Infrastructure.Database;
using Infrastructure.Storage;
using Microsoft.AspNetCore.Connections;
using Minio;
using Minio.AspNetCore;
using Npgsql;

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
            builder.Services.AddSwaggerGen();
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
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
