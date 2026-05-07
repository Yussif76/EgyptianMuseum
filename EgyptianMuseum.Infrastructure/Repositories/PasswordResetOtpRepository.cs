using EgyptianMuseum.Application.Interfaces;
using EgyptianMuseum.Domain.Entities;
using EgyptianMuseum.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EgyptianMuseum.Infrastructure.Repositories
{
    public class PasswordResetOtpRepository : IPasswordResetOtpRepository
    {
        private readonly AppDbContext _dbContext;

        public PasswordResetOtpRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(PasswordResetOtp otp)
        {
            await _dbContext.PasswordResetOtps.AddAsync(otp);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<PasswordResetOtp?> GetByUserIdAndCodeAsync(string userId, string code)
        {
            return await _dbContext.PasswordResetOtps
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Code == code && !x.IsUsed);
        }

        public async Task<PasswordResetOtp?> GetLatestByUserIdAsync(string userId)
        {
            return await _dbContext.PasswordResetOtps
                .Where(x => x.UserId == userId && !x.IsUsed)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(PasswordResetOtp otp)
        {
            _dbContext.PasswordResetOtps.Update(otp);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<PasswordResetOtp?> GetByEmailAndCodeAsync(string email, string code)
        {
            return await _dbContext.PasswordResetOtps
                .Include(x => x.User)
                .Where(x => x.User.Email == email && x.Code == code && !x.IsUsed)
                .FirstOrDefaultAsync();
        }
    }
}
