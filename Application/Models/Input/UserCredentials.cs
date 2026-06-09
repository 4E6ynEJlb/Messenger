namespace Application.Models.Input
{
    /// <summary>
    /// user auth data
    /// </summary>
    public record UserCredentials
    {
        /// <summary>
        /// 8-16 characters, only letters, digits and underscores, must start with a letter
        /// </summary>
        public required string Login { get; init; }
        /// <summary>
        /// 8-16 characters
        /// </summary>
        public required string Password { get; init; }
    }
}
