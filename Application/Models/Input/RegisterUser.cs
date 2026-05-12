namespace Application.Models.Input
{
    /// <summary>
    /// Model for user registration
    /// </summary>
    public record RegisterUser : UpdateUser
    {
        /// <summary>
        /// Length should be between 8 and 16 chars
        /// </summary>
        public required string UserLogin { get; init; }
        /// <summary>
        /// Length should be between 8 and 16 chars
        /// </summary>
        public required string UserPassword { get; init; }

    }
}