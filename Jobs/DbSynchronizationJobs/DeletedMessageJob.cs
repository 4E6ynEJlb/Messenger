using Domain.Models.Documents;
using Domain.Stores;
using Domain.Stores.MongoDB;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Jobs.DbSynchronizationJobs
{
    public class DeletedMessageJob : IJob
    {
        private IScheduler? _scheduler;
        private readonly ILogger _logger;
        private readonly IDeletedMessageStore _deletedMessageStore;
        private readonly ISynchronizationStore _synchronizationStore;
        public DeletedMessageJob(ILogger<DeletedMessageJob> logger,
            IDeletedMessageStore deletedMessageStore, ISynchronizationStore synchronizationStore)
        {
            _logger = logger;
            _deletedMessageStore = deletedMessageStore;
            _synchronizationStore = synchronizationStore;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _scheduler = context.Scheduler;
            int count = 0;
            try
            {
                DeletedMessage? message = await _deletedMessageStore.GetOneEldestAsync(context.CancellationToken);
                while (message is not null)
                {
                    switch (message.ChatType)
                    {
                        case Domain.Models.Types.EnChatType.Public:
                            await _synchronizationStore.DeletePublicMessageAsync(message.ChatId, 
                                message.MessageId, message.DeletedBy, 
                                message.DeletedAt, CancellationToken.None);
                            break;
                        case Domain.Models.Types.EnChatType.Personal:
                            await _synchronizationStore.DeletePersonalMessageAsync(message.ChatId,
                                message.MessageId, message.DeletedBy, 
                                CancellationToken.None);
                            break;
                    }

                    count++;
                    await _deletedMessageStore.DeleteAsync(message.MessageId, CancellationToken.None);
                    message = await _deletedMessageStore.GetOneEldestAsync(context.CancellationToken);
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
                _logger.LogInformation($"{count} messages deleted");
            else
                _logger.LogDebug("No messages deleted");
        }
    }
}
