using EgyptianMuseum.Application.DTOs.Auth;

namespace EgyptianMuseum.Application.Services.Auth
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterRequestDto dto);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
        Task ForgotPasswordAsync(ForgotPasswordRequestDto request);
        Task<bool> VerifyOtpAsync(VerifyOtpRequestDto request);
        Task ResetPasswordAsync(ResetPasswordRequestDto request);
        Task<AuthResponseDto> RefreshTokenAsync(  RefreshTokenRequestDto dto);
        Task LogoutAsync(string refreshToken);
        Task<AuthResponseDto> GoogleLoginAsync(GoogleLoginRequestDto dto);
        Task<bool> ConfirmEmailAsync(string userId, string token);

        /// <summary>
        /// Updates the display name (Name field) for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="request">The request containing the new display name.</param>
        /// <remarks>
        /// This method updates only the user's display name (Name field).
        /// It does NOT modify UserName, NormalizedUserName, Email, or Password.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when display name validation fails.</exception>
        /// <exception cref="InvalidOperationException">Thrown when user not found or update fails.</exception>
        Task ChangeUserNameAsync(string userId, ChangeUserNameRequestDto request);
    }
}

