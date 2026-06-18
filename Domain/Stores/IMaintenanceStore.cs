namespace Domain.Stores
{
    public interface IMaintenanceStore
    {
        public Task<Guid[]> ClearDeletedMediaAsync(CancellationToken cancellationToken);
        public Task<int> DeleteDesolatedChatsAsync(CancellationToken cancellationToken);
        public Task UnbanUsersBySentenceTimeAsync(CancellationToken cancellationToken);
        public Task DeleteExpireddRefreshTokensAsync(CancellationToken cancellationToken);
        public Task CreateLogTableForCurrentMonthAsync(CancellationToken cancellationToken);
        public Task DeleteLogTablesAsync(int expirationMonths, CancellationToken cancellationToken);
    }
}
