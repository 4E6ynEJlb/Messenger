using Domain.Models.Documents;
using Domain.Stores;
using Domain.Stores.MongoDB;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Jobs.DbSynchronizationJobs
{
    public class DeletedAttachmentJob : IJob
    {
        private IScheduler? _scheduler;
        private readonly ILogger _logger;
        private readonly IDeletedAttachmentStore _deletedAttachmentStore;
        private readonly ISynchronizationStore _synchronizationStore;
        public DeletedAttachmentJob(ILogger<DeletedAttachmentJob> logger,
            IDeletedAttachmentStore deletedAttachmentStore, ISynchronizationStore synchronizationStore)
        {
            _logger = logger;
            _deletedAttachmentStore = deletedAttachmentStore;
            _synchronizationStore = synchronizationStore;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _scheduler = context.Scheduler;
            int count = 0;
            try
            {
                DeletedAttachment? attachment = await _deletedAttachmentStore.GetOneEldestAsync(context.CancellationToken);
                while (attachment is not null)
                {
                    switch (attachment.Id.ChatType)
                    {
                        case Domain.Models.Types.EnChatType.Public:
                            await _synchronizationStore.DeleteFileFromPublicMessageAsync(attachment.Id.ChatId,
                                attachment.Id.MessageId, attachment.Id.MediaId, 
                                attachment.DeletedBy, attachment.DeletedAt, 
                                CancellationToken.None);
                            break;
                        case Domain.Models.Types.EnChatType.Personal:
                            await _synchronizationStore.DeleteFileFromPersonalMessageAsync(attachment.Id.ChatId,
                                attachment.Id.MessageId, attachment.Id.MediaId, 
                                attachment.DeletedBy, CancellationToken.None);
                            break;
                    }

                    count++;
                    await _deletedAttachmentStore.DeleteAsync(attachment.Id, CancellationToken.None);
                    attachment = await _deletedAttachmentStore.GetOneEldestAsync(context.CancellationToken);
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
                _logger.LogInformation($"{count} attachments deleted");
            else
                _logger.LogDebug("No attachments deleted");
        }
    }
}
