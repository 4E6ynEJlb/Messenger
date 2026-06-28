using Domain.Stores;
using Domain.Stores.MongoDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence;
using Persistence.Repositories;

namespace Maintenance.Extensions
{
    public static class ServicesConfigurationExtensions
    {
        public static void ConfigureServices(this HostApplicationBuilder builder)
        {
            builder.Services.AddScoped<IGenericChatStore, GenericChatRepository>();
            builder.Services.AddScoped<ISynchronizationStore, SynchronizationRepository>();
            builder.Services.AddScoped<IMaintenanceStore, MaintenanceRepository>();
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
