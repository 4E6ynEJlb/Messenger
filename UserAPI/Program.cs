using Microsoft.AspNetCore.Http.Features;
using UserAPI.Extensions;
using Domain.Stores;
using Persistence.Repositories;
using UserAPI.Services;
using UserAPI.Services.Interfaces;
using UserAPI.Hubs;

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
            builder.Configuration.AddJsonFile("infrastructureoptions.json");

            builder.ConfigureDatabaseConnectionFactory();
            builder.ConfigureMinioStorage();

            // Persistence repositories
            builder.Services.AddScoped<IUserStore, UserRepository>();
            builder.Services.AddScoped<IBotChatStore, BotChatRepository>();
            builder.Services.AddScoped<IBotControlStore, BotControlRepository>();
            builder.Services.AddScoped<IPersonalChatStore, PersonalChatRepository>();
            builder.Services.AddScoped<IPublicChatStore, PublicChatRepository>();
            builder.Services.AddScoped<IRefreshTokenStore, RefreshTokenRepository>();
            builder.Services.AddScoped<ISecurityStore, SecurityRepository>();

            // Updates service and hub
            builder.Services.AddScoped<UpdatesHub>();
            builder.Services.AddScoped<IUpdatesService, UpdatesService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.ConfigureSwaggerGen();
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Documentation.xml"));
            });

            builder.AddAuthBuilderExtension();
            builder.Services.AddSignalR();

            var app = builder.Build();
            app.AddAuthAppExtension();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();


            app.MapControllers();
            app.MapHub<UpdatesHub>("/updates");
            app.Run();
        }
    }
}
