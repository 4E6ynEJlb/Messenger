using Domain.Stores.MongoDB;
using Infrastructure.Database;
using Infrastructure.Storage;
using MediaAPI;
using MediaAPI.Middleware;
using Minio;
using Minio.AspNetCore;
using MongoDB.Driver;
using Npgsql;
using Persistence.Repositories;
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

            builder.ConfigureInfrastructure();
            builder.ConfigureLogging();
            builder.ConfigureServices();
            
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
