using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgyptianMuseum.Application.DTOs.Auth
{
    public class GoogleUserInfoDto
    {
        public string GoogleId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
