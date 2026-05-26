using Microsoft.AspNetCore.Http.Features;
using Prometheus;
using UserAPI.Extensions;
using UserAPI.Hubs;
using UserAPI.Services;
using Microsoft.AspNetCore.HttpOverrides;

namespace UserAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = 2L * 1024 * 1024 * 1024;
            });
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 2L * 1024 * 1024 * 1024;
                options.MemoryBufferThreshold = 1 * 1024 * 1024;
            });

            builder.Configuration.AddJsonFile("InfrastructureOptions.json");
            builder.Configuration.AddJsonFile("ApplicationOptions.json");

            builder.ConfigureOptions();
            builder.ConfigureLogging();
            builder.ConfigureDatabaseConnectionFactory();
            builder.ConfigureMinioStorage();
            builder.ConfigureRedis();
            builder.ConfigureBus();


            builder.ConfigureRepositories();
            builder.ConfigureServices();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.ConfigureSwaggerGen();
                options.AddServer(new Microsoft.OpenApi.Models.OpenApiServer
                {
                    Url = "/userapi"
                });
                options.SupportNonNullableReferenceTypes();
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "ApiDocumentation.xml"));
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "ApplicationDocumentation.xml"));
            });

            builder.AddAuthBuilderExtension();
            builder.Services.AddSignalR();

            var app = builder.Build();
            app.AddAuthAppExtension();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(c=>c.RouteTemplate = "swagger/{documentName}/swagger.json");
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/userapi/swagger/v1/swagger.json", "User API V1");
                    c.RoutePrefix = "swagger";
                });
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders =
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto
            });

            app.UseMiddleware<TelemetryMiddleware>();
            app.UseMiddleware<LoggingMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>();


            app.MapMetrics();
            app.MapControllers();
            app.MapHub<UpdatesHub>("/updates");
            app.Run();
        }
    }
}
