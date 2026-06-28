using Domain.Models.Documents;
using Domain.Stores;
using Domain.Stores.MongoDB;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Jobs.DbSynchronizationJobs
{
    public class DeletedChatJob : IJob
    {
        private IScheduler? _scheduler;
        private readonly ILogger _logger;
        private readonly IDeletedAttachmentStore _deletedAttachmentStore;
        private readonly IDeletedChatStore _deletedChatStore;
        private readonly IDeletedMessageStore _deletedMessageStore;
        private readonly IMessageUpdateStore _messageUpdateStore;
        private readonly INewMessageStore _newMessageStore;
        public DeletedChatJob(ILogger<DeletedChatJob> logger, 
            IDeletedAttachmentStore deletedAttachmentStore, IDeletedChatStore deletedChatStore, 
            IDeletedMessageStore deletedMessageStore, IMessageUpdateStore messageUpdateStore, 
            INewMessageStore newMessageStore)
        {
            _logger = logger;
            _deletedAttachmentStore = deletedAttachmentStore;
            _deletedChatStore = deletedChatStore;
            _deletedMessageStore = deletedMessageStore;
            _messageUpdateStore = messageUpdateStore;
            _newMessageStore = newMessageStore;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _scheduler = context.Scheduler;
            int count = 0;
            try
            {
                DeletedChat? chat = await _deletedChatStore.GetOneEldestAsync(context.CancellationToken);
                while (chat is not null)
                {
                    await _deletedAttachmentStore.DeleteByChatAsync(chat.Id, CancellationToken.None);
                    await _deletedMessageStore.DeleteByChatAsync(chat.Id, CancellationToken.None);
                    await _newMessageStore.DeleteByChatAsync(chat.Id, CancellationToken.None);
                    if(chat.Id.ChatType == Domain.Models.Types.EnChatType.Public)
                        await _messageUpdateStore.DeleteByChatAsync(chat.Id.ChatId, CancellationToken.None);

                    await _deletedChatStore.DeleteAsync(chat.Id.ChatId, chat.Id.ChatType, CancellationToken.None);

                    count++;
                    chat = await _deletedChatStore.GetOneEldestAsync(context.CancellationToken);
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
                _logger.LogInformation($"Collections cleaned up from {count} chats");
            else
                _logger.LogDebug("No collections cleaned up");
        }
    }
}
