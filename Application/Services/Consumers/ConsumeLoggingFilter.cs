using MassTransit;
using Microsoft.Extensions.Logging;

namespace Application.Services.Consumers
{
    public class ConsumeLoggingFilter<T> : IFilter<ConsumeContext<T>> where T : class
    {
        private readonly ILogger<ConsumeLoggingFilter<T>> _logger;
        public ConsumeLoggingFilter(ILogger<ConsumeLoggingFilter<T>> logger)
        {
            _logger = logger;
        }

        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            try
            {
                await next.Send(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kafka consume error: {MessageType}", typeof(T).Name);
                throw;
            }
        }

        public void Probe(ProbeContext context) { }
    }
}
