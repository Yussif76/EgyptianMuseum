using EgyptianMuseum.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken refreshToken);

        Task<RefreshToken?> GetByTokenAsync(string token);

        Task UpdateAsync(RefreshToken refreshToken);
    }
}
