using Application.Models.ConstantsAndExtensions;
using Application.Models.Internal.Constants;
using Application.Models.Internal.Options;
using Application.Models.Output;
using Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace Application.Services.Implementations
{
    public class MessageCacheService : IMessageCacheService
    {
        private readonly IDatabase _database;
        private readonly TimeSpan _expiration;
        public MessageCacheService([FromKeyedServices(CacheKeys.MESSAGES)] IConnectionMultiplexer connection, IOptions<ExpirationOptions> options)
        {
            _database = connection.GetDatabase();
            _expiration = TimeSpan.FromSeconds(options.Value.MessagesExpirationSeconds);
        }

        public async Task<Message?> GetAsync(Guid messageId, Guid chatId, CancellationToken cancellationToken)
        {
            string key = $"{messageId}{chatId}";
            RedisValue value = await _database.StringGetAsync(key);
            if (!value.HasValue)
                return null;
            string valueString = value.ToString();
            if (JsonSerializerHelper.TryDeserialize(valueString, out Message? message))
            {
                await _database.KeyExpireAsync(key, _expiration);
                return message;
            }
            return null;
        }

        public async Task InvalidateAsync(Guid messageId, Guid chatId, CancellationToken cancellationToken)
        {
            await _database.KeyDeleteAsync($"{messageId}{chatId}");
        }

        public async Task SaveAsync(Message message, CancellationToken cancellationToken)
        {
            string value = JsonSerializer.Serialize(message);
            await _database.StringSetAsync($"{message.MessageId}{message.ChatId}", value, new Expiration(_expiration));
        }
    }
}
