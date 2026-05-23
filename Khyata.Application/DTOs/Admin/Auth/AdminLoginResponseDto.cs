using Khyata.Application.DTOs.Admin.AdminUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.DTOs.Admin.Auth
{
    public class AdminLoginResponseDto
    {
        public string AccessToken { get; set; } = default!;
        public string TokenType { get; set; } = default!;
        public int ExpiresIn { get; set; }
        public AdminProfileDto Profile { get; set; } = default!;
    }
}
