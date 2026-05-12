namespace Application.Models.Input
{
    /// <summary>
    /// Model for updating user information. For not updating fields send the same values as before, 
    /// but for updating fields send new values.
    /// </summary>
    public record UpdateUser
    {
        /// <summary>
        /// Should be not null, not empty or whitespace, max length 32 chars
        /// </summary>
        public required string FirstName { get; init; }
        /// <summary>
        /// Can be null, max length 32 chars
        /// </summary>
        public required string? LastName { get; init; }
        /// <summary>
        /// Should be not null, not empty or whitespace, max length 16 chars
        /// </summary>
        public required string Tag { get; init; }
        public required DateOnly BirthDate { get; init; }
        /// <summary>
        /// Can be null, max length 512 chars
        /// </summary>
        public required string? Bio { get; init; }
    }
}
