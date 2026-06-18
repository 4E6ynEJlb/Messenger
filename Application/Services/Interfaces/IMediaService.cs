using Application.Models.Output;

namespace Application.Services.Interfaces
{
    public interface IMediaService
    {
        public Task<MediaInfo> GetFileInfo(Guid id, CancellationToken cancellationToken);
        public Task<MediaStream> GetFile(Guid id, CancellationToken cancellationToken);
    }
}
