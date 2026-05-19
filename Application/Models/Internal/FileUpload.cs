using Domain.Models.Types;

namespace Application.Models.Internal
{
    public record FileUpload
    {
        public required string FileName { get; init; }
        public required string ContentType { get; init; }
        public required Stream Content { get; init; }
        public required long Length { get; init; }
        internal MediaFile ToMediaFile(Guid mediaId) => new MediaFile()
        {
            MediaId = mediaId,
            FileName = FileName,
            ContentType = ContentType
        };
    }
}
