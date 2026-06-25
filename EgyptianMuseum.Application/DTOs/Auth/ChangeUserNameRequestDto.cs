namespace EgyptianMuseum.Application.DTOs.Auth
{
    /// <summary>
    /// Request DTO for updating the user's display name.
    /// </summary>
    public class ChangeUserNameRequestDto
    {
        /// <summary>
        /// The new display name for the user.
        /// Must be between 3 and 100 characters.
        /// </summary>
        public string NewUserName { get; set; } = null!;
    }
}
