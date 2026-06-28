using Domain.Models.Documents;
using Domain.Stores;
using Domain.Stores.MongoDB;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Jobs.DbSynchronizationJobs
{
    public class MessagesUpdatesJob : IJob
    {
        private IScheduler? _scheduler;
        private readonly ILogger _logger;
        private readonly ISynchronizationStore _synchronizationStore;
        private readonly IMessageUpdateStore _messageUpdateStore;
        public MessagesUpdatesJob(ILogger<MessagesUpdatesJob> logger, 
            ISynchronizationStore synchronizationStore, IMessageUpdateStore messageUpdateStore)
        {
            _logger = logger;
            _synchronizationStore = synchronizationStore;
            _messageUpdateStore = messageUpdateStore;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _scheduler = context.Scheduler;
            int count = 0;
            try
            {
                PublicMessageUpdate? update = await _messageUpdateStore.GetOneEldestAsync(context.CancellationToken);
                while (update is not null)
                {
                    await _synchronizationStore.LogPublicMessageUpdateAsync(update.ChatId, update.UpdatedBy, update.UpdatedAt, context.CancellationToken);
                    await _messageUpdateStore.DeleteAsync(update.UpdateId, CancellationToken.None);
                    count++;
                    update = await _messageUpdateStore.GetOneEldestAsync(context.CancellationToken);
                }
                LogResult(count);
                await ScheduleNextAsync(60, context.JobDetail, context.CancellationToken);
            }
            catch (TaskCanceledException tce)
            {
                LogResult(count);
                _logger.LogWarning(tce, "Job cancelled");
            }
            catch (Exception ex)
            {
                LogResult(count);
                _logger.LogError(ex, "Job execution error");
                await ScheduleNextAsync(5, context.JobDetail, context.CancellationToken);
            }
        }

        private async Task ScheduleNextAsync(int seconds, IJobDetail jobDetail, CancellationToken cancellationToken)
        {
            ITrigger trigger = TriggerBuilder.Create()
                .ForJob(jobDetail)
                .StartAt(
                    DateBuilder.FutureDate(
                        seconds,
                        IntervalUnit.Second))
                .Build();

            await _scheduler!.ScheduleJob(trigger);
        }

        private void LogResult(int count)
        {
            if (count > 0)
                _logger.LogInformation($"Saved {count} message updates");
            else
                _logger.LogDebug("No updates saved");
        }
    }
}
