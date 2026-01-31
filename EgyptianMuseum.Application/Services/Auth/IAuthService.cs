using EgyptianMuseum.Application.DTOs.Auth;

namespace EgyptianMuseum.Application.Services.Auth
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterRequestDto dto);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);
    }
}
