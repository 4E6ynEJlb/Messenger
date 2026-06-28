using Microsoft.Extensions.Hosting;
using Quartz;
using Jobs;
using Jobs.DbSynchronizationJobs;

namespace Maintenance.Extensions
{
    public static class JobConfigurationExtensions
    {
        public static void ConfigureJobs(this HostApplicationBuilder builder)
        {
            builder.Services.AddQuartz(options =>
            {
                JobKey jobKey = new JobKey("DeleteDesolatedChatsJob");
                options.AddJob<DeleteDesolatedChatsJob>(jobKey);
                options.AddTrigger(triggerOptions =>
                {
                    triggerOptions.ForJob(jobKey)
                    .WithIdentity("DeleteDesolatedChatsJobTrigger")
                    .WithSimpleSchedule(schedule => schedule.WithIntervalInMinutes(1).RepeatForever());
                });
            });

            builder.Services.AddQuartz(options =>
            {
                JobKey jobKey = new JobKey("ClearDeletedMediaJob");
                options.AddJob<ClearDeletedMediaJob>(jobKey);
                options.AddTrigger(triggerOptions =>
                {
                    triggerOptions.ForJob(jobKey)
                    .WithIdentity("ClearDeletedMediaJobTrigger")
                    .WithSimpleSchedule(schedule => schedule.WithIntervalInMinutes(1).RepeatForever());
                });
            });

            builder.Services.AddQuartz(options =>
            {
                JobKey jobKey = new JobKey("DeleteExpiredRefreshTokensJob");
                options.AddJob<DeleteExpiredRefreshTokensJob>(jobKey);
                options.AddTrigger(triggerOptions =>
                {
                    triggerOptions.ForJob(jobKey)
                    .WithIdentity("DeleteExpiredRefreshTokensJobTrigger")
                    .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(5).RepeatForever());
                });
            });

            builder.Services.AddQuartz(options =>
            {
                JobKey jobKey = new JobKey("MessagesUpdatesJob");
                options.AddJob<MessagesUpdatesJob>(jobKey);
                options.AddTrigger(triggerOptions =>
                {
                    triggerOptions.ForJob(jobKey)
                    .WithIdentity("MessagesUpdatesJobTrigger")
                    .StartNow();
                });
            });

            builder.Services.AddQuartz(options =>
            {
                JobKey jobKey = new JobKey("NewMessagesJob");
                options.AddJob<NewMessagesJob>(jobKey);
                options.AddTrigger(triggerOptions =>
                {
                    triggerOptions.ForJob(jobKey)
                    .WithIdentity("NewMessagesJobTrigger")
                    .StartNow();
                });
            });

            builder.Services.AddQuartz(options =>
            {
                JobKey jobKey = new JobKey("DeletedChatJob");
                options.AddJob<DeletedChatJob>(jobKey);
                options.AddTrigger(triggerOptions =>
                {
                    triggerOptions.ForJob(jobKey)
                    .WithIdentity("DeletedChatJobTrigger")
                    .StartNow();
                });
            });

            builder.Services.AddQuartz(options =>
            {
                JobKey jobKey = new JobKey("DeletedMessageJob");
                options.AddJob<DeletedMessageJob>(jobKey);
                options.AddTrigger(triggerOptions =>
                {
                    triggerOptions.ForJob(jobKey)
                    .WithIdentity("DeletedMessageJobTrigger")
                    .StartNow();
                });
            });

            builder.Services.AddQuartz(options =>
            {
                JobKey jobKey = new JobKey("DeletedAttachmentJob");
                options.AddJob<DeletedAttachmentJob>(jobKey);
                options.AddTrigger(triggerOptions =>
                {
                    triggerOptions.ForJob(jobKey)
                    .WithIdentity("DeletedAttachmentJobTrigger")
                    .StartNow();
                });
            });
        }
    }
}
