using Domain.Models.Documents;
using Domain.Models.Types;
using Domain.Stores;
using Domain.Stores.MongoDB;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Jobs.DbSynchronizationJobs
{
    public class NewMessagesJob : IJob
    {
        private IScheduler? _scheduler;
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISynchronizationStore _synchronizationStore;
        private readonly INewMessageStore _newMessageStore;
        private readonly INewMediaStore _newMediaStore;
        public NewMessagesJob(ILogger<NewMessagesJob> logger,
            IUnitOfWork unitOfWork, INewMessageStore newMessageStore, INewMediaStore newMediaStore)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _synchronizationStore = unitOfWork.Synchronization;
            _newMessageStore = newMessageStore;
            _newMediaStore = newMediaStore;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _scheduler = context.Scheduler;
            int messagesCount = 0,
                mediaCount = 0;
            try
            {
                await _unitOfWork.BeginTransactionAsync(context.CancellationToken);
                NewMessage? message = await _newMessageStore.GetOneEldestAsync(context.CancellationToken);
                while (message is not null)
                {
                    List<NewMedia> newMediaList = await _newMediaStore.GetManyByIdAsync(message.AttachedMedia, context.CancellationToken);
                    AttachmentInput[] attachments = new AttachmentInput[message.AttachedMedia.Count];
                    Dictionary<Guid, int> links = newMediaList.ToDictionary(k => k.MediaId, v => v.LinksCount);

                    for (int i = 0; i < attachments.Length; i++)
                    {
                        NewMedia? newMedia = newMediaList.FirstOrDefault(m => m.MediaId == message.AttachedMedia[i]);
                        AttachmentInput attachmentInput;
                        if (newMedia is not null)
                            attachmentInput = new AttachmentInput()
                            {
                                ContentType = newMedia.ContentType,
                                LinksCount = newMedia.LinksCount,
                                FileName = newMedia.FileName,
                                MediaId = newMedia.MediaId
                            };
                        else
                            attachmentInput = new AttachmentInput()
                            {
                                MediaId = message.AttachedMedia[i],
                                ContentType = null,
                                LinksCount = null,
                                FileName = null
                            };

                        attachments[i] = attachmentInput;
                    }

                    MessageInput messageInput = new MessageInput()
                    {
                        MessageId = message.MessageId,
                        Author = message.Author,
                        IsBot = message.IsBot,
                        MessageText = message.MessageText,
                        SentAt = message.SentAt,
                        IsBotResend = message.IsBotResend,
                        IsUpdated = message.IsUpdated,
                        UpdatedAt = message.UpdatedAt,
                        ReplyTo = message.ReplyTo,
                        ResentFrom = message.ResentFrom,
                        AttachedMedia = attachments
                    };

                    await _synchronizationStore.SaveMessageAsync(message.ChatType, message.ChatId, messageInput, context.CancellationToken);

                    List<NewMedia> checkMediaList = await _newMediaStore.GetManyByIdAsync(message.AttachedMedia, context.CancellationToken);
                    bool correct = true;
                    foreach (var check in checkMediaList)
                        if (!links.TryGetValue(check.MediaId, out int val) || val != check.LinksCount)
                        {
                            correct = false;
                            break;
                        }

                    if (correct)
                    { 
                        await _unitOfWork.CommitTransactionAsync(CancellationToken.None);

                        NewMessage? checkMessage = await _newMessageStore.GetOneByIdAsync(message.MessageId, CancellationToken.None);
                        if (checkMessage is not null && checkMessage.UpdatedAt != message.UpdatedAt)
                            await _synchronizationStore.UpdateMessageAsync(checkMessage.ChatType,
                                checkMessage.ChatId, checkMessage.MessageId, checkMessage.MessageText,
                                checkMessage.UpdatedAt ?? DateTime.UtcNow, CancellationToken.None);

                        await _newMessageStore.DeleteAsync(message.MessageId, CancellationToken.None);
                        await _newMediaStore.DeleteManyAsync(links.Keys.ToList(), CancellationToken.None);
                        messagesCount++;
                        mediaCount += links.Count;
                    }
                    else
                        await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);

                    message = await _newMessageStore.GetOneEldestAsync(context.CancellationToken);
                }

                LogResult(messagesCount, mediaCount);
                await ScheduleNextAsync(60, context.JobDetail, context.CancellationToken);
            }
            catch (TaskCanceledException tce)
            {
                LogResult(messagesCount, mediaCount);
                _logger.LogWarning(tce, "Job cancelled");
                await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                LogResult(messagesCount, mediaCount);
                _logger.LogError(ex, "Job execution error");
                await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);
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

        private void LogResult(int messagesCount, int mediaCount)
        {
            if (messagesCount > 0)
                _logger.LogInformation($"Saved {messagesCount} messages, {mediaCount} media");
            else
                _logger.LogDebug("No messages saved");
        }
    }
}
