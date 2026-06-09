using Application.Models.Internal;

namespace UserAPI.Extensions
{
    public static class IFormFileExtensions
    {
        public static FileUpload ToFileUpload(this IFormFile file)
        {
            return new FileUpload()
            {
                FileName = file.FileName,
                Content = file.OpenReadStream(),
                ContentType = file.ContentType,
                Length = file.Length
            };
        }
    }
}
