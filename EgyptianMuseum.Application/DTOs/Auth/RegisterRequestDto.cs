namespace EgyptianMuseum.Application.DTOs.Auth
{
    public class RegisterRequestDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Name { get; set; }
        public string Language { get; set; } = "en";
    }
}
