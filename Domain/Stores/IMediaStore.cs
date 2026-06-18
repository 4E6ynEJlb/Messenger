using Domain.Models.Types;

namespace Domain.Stores
{
    public interface IMediaStore
    {
        public Task<MediaFile?> GetOneByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
