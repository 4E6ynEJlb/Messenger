using Domain.Stores;
using Infrastructure.Storage;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Jobs
{
    public class ClearDeletedMediaJob : IJob
    {
        private readonly ILogger _logger;
        private readonly IObjectStorage _objectStorage;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMaintenanceStore _maintenanceStore;
        public ClearDeletedMediaJob(ILogger<ClearDeletedMediaJob> logger, IObjectStorage objectStorage, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _objectStorage = objectStorage;
            _unitOfWork = unitOfWork;
            _maintenanceStore = _unitOfWork.Maintenance;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync(context.CancellationToken);

                Guid[] media = await _maintenanceStore.ClearDeletedMediaAsync(context.CancellationToken);
                if(media.Length > 0)
                    await _objectStorage.DeleteManyAsync(media, context.CancellationToken);

                await _unitOfWork.CommitTransactionAsync(CancellationToken.None);

                int count = media.Length;
                if(count > 0)
                    _logger.LogInformation($"Deleted {count} unreachable media");
                else
                    _logger.LogDebug("Unreachable media not found");
            }
            catch (TaskCanceledException tce)
            {
                await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                _logger.LogWarning(tce, "Job cancelled");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);
                _logger.LogError(ex, "Job execution error");
            }
        }
    }
}
