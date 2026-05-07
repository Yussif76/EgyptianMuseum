using EgyptianMuseum.Application.DTOs.Auth;
using EgyptianMuseum.Application.Interfaces;
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
        private readonly IEmailService _emailService;
        private readonly IPasswordResetOtpRepository _otpRepository;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IEmailService emailService,
            IPasswordResetOtpRepository otpRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
            _otpRepository = otpRepository;
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

        public async Task ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            // Always return success to prevent user enumeration
            if (user == null)
            {
                return;
            }

            // Generate 6-digit OTP
            var otp = GenerateOtp();
            var expiryTime = DateTime.UtcNow.AddMinutes(10);

            var passwordResetOtp = new PasswordResetOtp
            {
                UserId = user.Id,
                Code = otp,
                ExpiryTime = expiryTime,
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            await _otpRepository.AddAsync(passwordResetOtp);

            // Send email with OTP
            var subject = "MuseWay - OTP Code";
            var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #8B4513; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 20px; border: 1px solid #ddd; border-radius: 0 0 5px 5px; }}
        .otp {{ font-size: 24px; font-weight: bold; color: #8B4513; text-align: center; padding: 20px; }}
        .footer {{ font-size: 12px; color: #999; margin-top: 20px; text-align: center; }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>MuseWay</h1>
        </div>
        <div class=""content"">
            <p>Hello {user.Name},</p>
            <p>You requested to reset your password. Here is your one-time password (OTP):</p>
            <div class=""otp"">{otp}</div>
            <p><strong>This OTP will expire in 10 minutes.</strong></p>
            <p>If you did not request this, please ignore this email.</p>
            <div class=""footer"">
                <p>© 2026 MuseWay. All rights reserved.</p>
            </div>
        </div>
    </div>
</body>
</html>";

            await _emailService.SendEmailAsync(request.Email, subject, body, isHtml: true);
        }

        public async Task<bool> VerifyOtpAsync(VerifyOtpRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            var otp = await _otpRepository.GetByUserIdAndCodeAsync(user.Id, request.Otp);

            if (otp == null)
            {
                throw new InvalidOperationException("Invalid OTP");
            }

            if (otp.IsUsed)
            {
                throw new InvalidOperationException("OTP has already been used");
            }

            if (DateTime.UtcNow > otp.ExpiryTime)
            {
                throw new InvalidOperationException("OTP has expired");
            }

            return true;
        }

        public async Task ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            var otp = await _otpRepository.GetByUserIdAndCodeAsync(user.Id, request.Otp);

            if (otp == null)
            {
                throw new InvalidOperationException("Invalid OTP");
            }

            if (otp.IsUsed)
            {
                throw new InvalidOperationException("OTP has already been used");
            }

            if (DateTime.UtcNow > otp.ExpiryTime)
            {
                throw new InvalidOperationException("OTP has expired");
            }

            // Reset password using UserManager
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Password reset failed: {errors}");
            }

            // Mark OTP as used
            otp.IsUsed = true;
            await _otpRepository.UpdateAsync(otp);
        }

        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
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
