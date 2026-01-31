namespace EgyptianMuseum.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string? Email { get; set; }
        public string? Name { get; set; }
    }
}
