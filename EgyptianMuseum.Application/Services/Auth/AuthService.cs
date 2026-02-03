using EgyptianMuseum.Application.DTOs.Auth;
using EgyptianMuseum.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EgyptianMuseum.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<string> RegisterAsync(RegisterRequestDto dto)
        {
            var userExists = await _userManager.FindByEmailAsync(dto.Email);
            if (userExists != null)
            {
                throw new InvalidOperationException("User with this email already exists");
            }

            var normalizedLanguage = dto.Language?.Trim().ToLower() ?? "en";
            if (normalizedLanguage != "en" && normalizedLanguage != "ar")
            {
                throw new InvalidOperationException("Unsupported language");
            }

            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email,
                Name = dto.Name,
                PreferredLanguage = normalizedLanguage
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"User creation failed: {errors}");
            }

            return user.Id;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                throw new InvalidOperationException("Invalid email or password");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Invalid email or password");
            }

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                Email = user.Email,
                Name = user.Name
            };
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            // prefer 'JWT' section (matches appsettings), fallback to 'Jwt'
            var jwtSection = _configuration.GetSection("JWT");
            if (!jwtSection.Exists()) jwtSection = _configuration.GetSection("Jwt");

            string? secret = jwtSection.GetValue<string>("SecretKey");
            string? issuer = jwtSection.GetValue<string>("Issuer");
            string? audience = jwtSection.GetValue<string>("Audience");
            string? expiryStr = jwtSection.GetValue<string>("ExpiryMinutes");


            if (string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException("JWT SecretKey is not configured.");

            if (!double.TryParse(expiryStr, out var expiryMinutes))
            {
                expiryMinutes = 60; // default
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("sub", user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            if (!string.IsNullOrWhiteSpace(user.Name))
            {
                claims.Add(new Claim(ClaimTypes.Name, user.Name));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
