using Application.Models.Input;
using Application.Models.Internal;
using Application.Models.Output;

namespace Application.Services.Interfaces
{
    public interface IUserService
    {
        public Task<User> GetUserAsync(Guid id, CancellationToken cancellationToken);
        public Task<User> GetUserByTagAsync(string tag, CancellationToken cancellationToken);
        public Task<string[]> GetAvatarsAsync(Guid id, CancellationToken cancellationToken);
        public Task<BanInformation?> GetBanInformationAsync(Guid id, CancellationToken cancellationToken);
        public Task<ChatShortInfo[]> GetChatsAsync(Guid id, PageOptions pageOptions, CancellationToken cancellationToken);
        public Task UpdateCredentialsAsync(Guid id, UpdateCredentials updateCredentials, CancellationToken cancellationToken);
        public Task UpdateProfileAsync(Guid id, UpdateUser updateUser, CancellationToken cancellationToken);
        public Task UploadAvatarAsync(Guid id, FileUpload file, CancellationToken cancellationToken);
        public Task DeleteAvatarAsync(Guid id, string mediaLink, CancellationToken cancellationToken);
        public Task DeleteAccountAsync(Guid id, string password, CancellationToken cancellationToken);
    }
}
