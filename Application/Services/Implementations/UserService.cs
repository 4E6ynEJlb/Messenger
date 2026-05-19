using Application.Models.Input;
using Application.Models.Internal;
using Application.Models.Internal.Options;
using Application.Models.Output;
using Application.Services.Interfaces;
using Domain.Models.Types;
using Domain.Models.Types.Tables;
using Domain.Stores;
using Infrastructure.Storage;
using Microsoft.Extensions.Options;

namespace Application.Services.Implementations
{
    internal class UserService : IUserService
    {
        private readonly string _mediaPrefix;
        private readonly IBotUserCacheService _cache;
        private readonly IUserStore _userStore;
        private readonly IObjectStorage _objectStorage;
        public UserService(IUserStore userStore, 
            IObjectStorage objectStorage, IBotUserCacheService cache, 
            IOptions<ApplicationServicesOptions> options)
        {
            _userStore = userStore;
            _objectStorage = objectStorage;
            _cache = cache;
            _mediaPrefix = options.Value.MediaPrefix;
        }

        public async Task DeleteAccountAsync(Guid id, string password, CancellationToken cancellationToken)
        {
            await _userStore.DeleteUserAsync(id, password, cancellationToken);
            await _cache.InvalidateAsync(id, cancellationToken);
        }

        public async Task DeleteAvatarAsync(Guid id, string mediaLink, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(mediaLink[(_mediaPrefix.Length + 1)..], out Guid mediaId))
                return;
            await _cache.InvalidateAsync(id, cancellationToken);
            await _userStore.DeleteUserAvatarAsync(id, mediaId, cancellationToken);
        }

        public async Task<string[]> GetAvatarsAsync(Guid id, CancellationToken cancellationToken)
        {
            Guid[] avatars = await _userStore.GetUserAvatarsAsync(id, cancellationToken);
            string[] mediaLinks = avatars.Select(a => $"{_mediaPrefix}/{a}").ToArray();
            return mediaLinks;
        }

        public async Task<BanInformation?> GetBanInformationAsync(Guid id, CancellationToken cancellationToken)
        {
            BannedUsers? banInfo = await _userStore.GetBannedUserInformationAsync(id, cancellationToken);
            if (banInfo is null)
                return null;
            BanInformation banInformation = new BanInformation(banInfo);
            return banInformation;
        }

        public async Task<ChatShortInfo[]> GetChatsAsync(Guid id, PageOptions pageOptions, CancellationToken cancellationToken)
        {
            ChatInformation[] chats = await _userStore.GetUserChatsAsync(id, pageOptions.Page, pageOptions.PageSize, cancellationToken);
            ChatShortInfo[] chatShortInfo = chats.Select(c => new ChatShortInfo(c, _mediaPrefix)).ToArray();
            return chatShortInfo;
        }

        public async Task<User> GetUserAsync(Guid id, CancellationToken cancellationToken)
        {
            User? user = await _cache.GetUserAsync(id, cancellationToken);
            if (user is null)
            {
                UserData userData = await _userStore.GetUserByIdAsync(id, cancellationToken);
                user = new User(userData, _mediaPrefix);
                await _cache.SaveUserAsync(user, cancellationToken);
            }
            return user;
        }

        public async Task<User> GetUserByTagAsync(string tag, CancellationToken cancellationToken)
        {
            UserData userData = await _userStore.GetUserByTagAsync(tag, cancellationToken);
            User user = new User(userData, _mediaPrefix);
            await _cache.SaveUserAsync(user, cancellationToken);
            return user;
        }

        public async Task UpdateCredentialsAsync(Guid id, UpdateCredentials updateCredentials, CancellationToken cancellationToken)
        {
            await _userStore.UpdateUserAuthAsync(updateCredentials.ToUpdateUserAuthModel(id), cancellationToken);
        }

        public async Task UpdateProfileAsync(Guid id, UpdateUser updateUser, CancellationToken cancellationToken)
        {
            UserData userData = updateUser.ToUserData(id);
            await _userStore.UpdateUserDataAsync(userData, cancellationToken);
        }

        public async Task UploadAvatarAsync(Guid id, FileUpload file, CancellationToken cancellationToken)
        {
            Guid mediaId = Guid.NewGuid();
            await _userStore.UploadUserAvatarAsync(id, file.ToMediaFile(mediaId), cancellationToken);
            await _objectStorage.SaveAsync(file.Content, mediaId, cancellationToken);
        }
    }
}
