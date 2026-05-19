using MassTransit;
using Microsoft.Extensions.Logging;

namespace Application.Services.Consumers
{
    public class FaultConsumeObserver : IConsumeObserver
    {
        private readonly ILogger<FaultConsumeObserver> _logger;
        public FaultConsumeObserver(ILogger<FaultConsumeObserver> logger)
        {
            _logger = logger;
        }
        public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
        {
            _logger.LogError(exception, $"Error on {context.Message.GetType().Name}");
            return Task.CompletedTask;
        }

        public Task PostConsume<T>(ConsumeContext<T> context) where T : class
        {
            return Task.CompletedTask;
        }

        public Task PreConsume<T>(ConsumeContext<T> context) where T : class
        {
            return Task.CompletedTask;
        }
    }
}
