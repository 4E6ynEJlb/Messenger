using Domain.Stores;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Jobs
{
    public class DeleteDesolatedChatsJob : IJob
    {
        private readonly ILogger _logger;
        private readonly IMaintenanceStore _maintenanceStore;
        public DeleteDesolatedChatsJob(ILogger<DeleteDesolatedChatsJob> logger, IMaintenanceStore maintenanceStore)
        {
            _logger = logger;
            _maintenanceStore = maintenanceStore;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                int count = await _maintenanceStore.DeleteDesolatedChatsAsync(context.CancellationToken);
                if (count > 0)
                    _logger.LogInformation($"Deleted {count} desolated chats");
                else
                    _logger.LogDebug("Desolated chats not found");
            }
            catch (TaskCanceledException tce)
            {
                _logger.LogWarning(tce, "Job cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Job execution error");
            }
        }
    }
}
