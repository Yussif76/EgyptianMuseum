using EgyptianMuseum.Domain.Entities;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface IPasswordResetOtpRepository
    {
        Task AddAsync(PasswordResetOtp otp);
        Task<PasswordResetOtp?> GetByUserIdAndCodeAsync(string userId, string code);
        Task<PasswordResetOtp?> GetLatestByUserIdAsync(string userId);
        Task UpdateAsync(PasswordResetOtp otp);
        Task<PasswordResetOtp?> GetByEmailAndCodeAsync(string email, string code);
    }
}
