using Application.Models.Internal.Messages;
using Application.Services.Interfaces;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Consumers
{
    public class FileDeletedConsumer : IConsumer<FileDeletedMessage>
    {
        private readonly IUpdatesService _updatesService;
        public FileDeletedConsumer(IUpdatesService updatesService)
        {
            _updatesService = updatesService;
        }
        public async Task Consume(ConsumeContext<FileDeletedMessage> context)
        {
            FileDeletedMessage message = context.Message;
            await _updatesService.FileDeleted(
                message.ChatId, message.File, message.MessageId, 
                message.UserId, message.ChatType, context.CancellationToken);
        }
    }
}
