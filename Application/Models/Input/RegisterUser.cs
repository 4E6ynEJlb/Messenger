namespace Application.Models.Input
{
    public record RegisterUser : UpdateUser
    {
        public required string UserLogin { get; init; }
        public required string UserPassword { get; init; }

    }
}