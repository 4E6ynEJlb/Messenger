namespace Application.Models.Input
{
    public record RegisterUser : UpdateUser
    {
        public required byte[] UserLogin { get; init; }
        public required byte[] UserPassword { get; init; }

    }
}