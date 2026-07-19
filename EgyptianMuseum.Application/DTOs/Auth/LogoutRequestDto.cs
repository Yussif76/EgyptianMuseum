using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Application.DTOs.Auth
{
    public class LogoutRequestDto
    {
        public string RefreshToken { get; set; } = null!;
    }
}
