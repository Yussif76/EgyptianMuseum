namespace EgyptianMuseum.Application.DTOs.Auth
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;
        public string? Name { get; set; }
        public string Language { get; set; } = "en";
    }
}
