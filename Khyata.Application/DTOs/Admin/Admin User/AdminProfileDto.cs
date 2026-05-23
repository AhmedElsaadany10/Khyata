using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.DTOs.Admin.AdminUser
{
    public class AdminProfileDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string[] Roles { get; set; } = default!;
        public bool? IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}
