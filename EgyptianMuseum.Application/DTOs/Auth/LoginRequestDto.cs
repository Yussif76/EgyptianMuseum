namespace EgyptianMuseum.Application.DTOs.Auth
{
    using System.ComponentModel.DataAnnotations;

    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
