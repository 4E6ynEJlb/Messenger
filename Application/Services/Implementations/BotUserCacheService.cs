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
    public class BotUserCacheService : IBotUserCacheService
    {
        private readonly IDatabase _database;
        private readonly TimeSpan _expiration;
        public BotUserCacheService([FromKeyedServices(CacheKeys.BOT_USER)]IConnectionMultiplexer connection, IOptions<ExpirationOptions> options)
        {
            _database = connection.GetDatabase();
            _expiration = TimeSpan.FromSeconds(options.Value.BotUserExpirationSeconds);
        }

        public async Task<Bot?> GetBotAsync(Guid id, CancellationToken cancellationToken)
        {
            RedisValue value = await _database.StringGetAsync(id.ToString());
            if (!value.HasValue)
                return null;
            string valueString = value.ToString();
            if (JsonSerializerHelper.TryDeserialize(valueString, out Bot? bot))
            {
                await _database.KeyExpireAsync(id.ToString(), _expiration);
                return bot; 
            }
            return null;
        }

        public async Task<User?> GetUserAsync(Guid id, CancellationToken cancellationToken)
        {
            RedisValue value = await _database.StringGetAsync(id.ToString());
            if (!value.HasValue)
                return null;
            string valueString = value.ToString();
            if (JsonSerializerHelper.TryDeserialize(valueString, out User? user))
            { 
                return user; 
            }
            return null;
        }

        public async Task InvalidateAsync(Guid id, CancellationToken cancellationToken)
        {
            await _database.KeyDeleteAsync(id.ToString());
        }

        public async Task SaveBotAsync(Bot bot, CancellationToken cancellationToken)
        {
            string value = JsonSerializer.Serialize(bot);
            await _database.StringSetAsync(bot.BotId.ToString(), value, new Expiration(_expiration));
        }

        public async Task SaveUserAsync(User user, CancellationToken cancellationToken)
        {
            string value = JsonSerializer.Serialize(user);
            await _database.StringSetAsync(user.UserId.ToString(), value, new Expiration(_expiration));
        }
    }
}
