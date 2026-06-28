using Domain.Stores;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Jobs
{
    public class DeleteExpiredRefreshTokensJob : IJob
    {
        private readonly ILogger _logger;
        private readonly IMaintenanceStore _maintenanceStore;
        public DeleteExpiredRefreshTokensJob(ILogger<DeleteExpiredRefreshTokensJob> logger, IMaintenanceStore maintenanceStore)
        {
            _logger = logger;
            _maintenanceStore = maintenanceStore;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await _maintenanceStore.DeleteExpireddRefreshTokensAsync(context.CancellationToken);
                _logger.LogDebug("Deleted expired refresh tokens");
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
