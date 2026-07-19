using EgyptianMuseum.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Application.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<GoogleUserInfoDto> ValidateTokenAsync(string idToken);
    }
}
