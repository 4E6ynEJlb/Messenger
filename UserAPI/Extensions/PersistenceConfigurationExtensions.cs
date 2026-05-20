using Domain.Stores;
using Infrastructure.Database;
using Npgsql;
using Persistence.Repositories;

namespace UserAPI.Extensions
{
    public static class PersistenceConfigurationExtensions
    {
        public static void ConfigureRepositories(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUserStore, UserRepository>();
            builder.Services.AddScoped<IBotChatStore, BotChatRepository>();
            builder.Services.AddScoped<IBotControlStore, BotControlRepository>();
            builder.Services.AddScoped<IPersonalChatStore, PersonalChatRepository>();
            builder.Services.AddScoped<IPublicChatStore, PublicChatRepository>();
            builder.Services.AddScoped<IRefreshTokenStore, RefreshTokenRepository>();
            builder.Services.AddScoped<ISecurityStore, SecurityRepository>();
        }
    }
}
