using Domain.Models.Documents;
using Domain.Models.Types;
using System.Diagnostics.CodeAnalysis;

namespace Application.Models.Internal
{
    public record MediaFileInfo
    {
        public MediaFileInfo() { }

        [SetsRequiredMembers]
        public MediaFileInfo(MediaFile mediaFile)
        {
            MediaId = mediaFile.MediaId;
            FileName = mediaFile.FileName;
            ContentType = mediaFile.ContentType;
        }

        [SetsRequiredMembers]
        public MediaFileInfo(NewMedia newMedia)
        {
            MediaId = newMedia.MediaId;
            FileName = newMedia.FileName;
            ContentType = newMedia.ContentType;
        }

        public required Guid MediaId { get; init; }
        public required string FileName { get; init; }
        public required string ContentType { get; init; }
    }
}
