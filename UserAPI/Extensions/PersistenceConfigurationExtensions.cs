using Domain.Stores;
using Domain.Stores.MongoDB;
using Infrastructure.Database;
using Npgsql;
using Persistence;
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
            builder.Services.AddScoped<IGenericChatStore, GenericChatRepository>();
            builder.Services.AddScoped<UnitOfWorkConnectionScope>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddSingleton<IDeletedAttachmentStore, DeletedAttachmentRepository>();
            builder.Services.AddSingleton<IDeletedChatStore, DeletedChatRepository>();
            builder.Services.AddSingleton<IDeletedMessageStore, DeletedMessageRepository>();
            builder.Services.AddSingleton<IMessageUpdateStore, MessageUpdateRepository>();
            builder.Services.AddSingleton<INewMediaStore, NewMediaRepository>();
            builder.Services.AddSingleton<INewMessageStore, NewMessageRepository>();
        }
    }
}
