using Application.Models.Internal.Options;
using Application.Services.Implementations;
using Application.Services.Interfaces;
using UserAPI.Services;

namespace UserAPI.Extensions
{
    public static class ApplicationConfiguration
    {
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUpdatesService, UpdatesService>();
            builder.Services.AddScoped<IMessagePublisher, MessagePublisher>();
            builder.Services.AddSingleton<IBotUserCacheService, BotUserCacheService>();
            builder.Services.AddSingleton<IMessageCacheService, MessageCacheService>();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IBotChatService, BotChatService>();
            builder.Services.AddScoped<IBotService, BotService>();
            builder.Services.AddScoped<IPersonalChatService, PersonalChatService>();
            builder.Services.AddScoped<IPublicChatService, PublicChatService>();
            builder.Services.AddScoped<ISecurityService, SecurityService>();
            builder.Services.AddScoped<IUserService, UserService>();
        }

        public static void ConfigureOptions(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<ApplicationServicesOptions>(
                builder.Configuration.GetSection(ApplicationServicesOptions.OPTIONS_NAME));
            builder.Services.Configure<ExpirationOptions>(
                builder.Configuration.GetSection(ExpirationOptions.OPTIONS_NAME));
            builder.Services.Configure<TokenExpirationOptions>(
                builder.Configuration.GetSection(TokenExpirationOptions.OPTIONS_NAME));
        }
    }
}
