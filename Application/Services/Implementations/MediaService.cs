using Application.Models.Internal;
using Application.Models.Output;
using Application.Services.Interfaces;
using Domain.Models.Documents;
using Domain.Models.Types;
using Domain.Stores;
using Domain.Stores.MongoDB;
using Infrastructure.Storage;

namespace Application.Services.Implementations
{
    public class MediaService : IMediaService
    {
        private readonly IMediaStore _mediaStore;
        private readonly INewMediaStore _newMediaStore;
        private readonly IObjectStorage _objectStorage;
        public MediaService(INewMediaStore newMediaStore, IMediaStore mediaStore, IObjectStorage objectStorage)
        {
            _mediaStore = mediaStore;
            _newMediaStore = newMediaStore;
            _objectStorage = objectStorage;
        }

        public async Task<MediaInfo> GetFileInfo(Guid id, CancellationToken cancellationToken)
        {
            MediaFileInfo? info = await GetFileFullInfo(id, cancellationToken);
            
            if (info is null)
                throw new FileNotFoundException();

            return new MediaInfo() 
                { 
                    ContentType = info.ContentType, 
                    FileName = info.FileName
                };
        }

        public async Task<MediaStream> GetFile(Guid id, CancellationToken cancellationToken)
        {
            MediaFileInfo? info = await GetFileFullInfo(id, cancellationToken);

            if (info is null)
                throw new FileNotFoundException();

            return new MediaStream()
            {
                ContentType = info.ContentType,
                FileName = info.FileName,
                Content = await _objectStorage.GetAsync(id, cancellationToken)
            };
        }

        private async Task<MediaFileInfo?> GetFileFullInfo(Guid id, CancellationToken cancellationToken)
        {
            NewMedia? mongoResult = await _newMediaStore.GetOneByIdAsync(id, cancellationToken);
            if (mongoResult is not null)
                return new MediaFileInfo(mongoResult);

            MediaFile? sqlResult = await _mediaStore.GetOneByIdAsync(id, cancellationToken);
            if (sqlResult is not null)
                return new MediaFileInfo(sqlResult);

            return null;
        }
    }
}
