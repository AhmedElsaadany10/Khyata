using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.DTOs.Admin.Auth
{
    public class RegisterAdminDto
    {
        [Required, MaxLength(100)]
        public string Username { get; set; } = default!;

        [Required, MaxLength(200)]
        public string DisplayName { get; set; } = default!;

        [Required, EmailAddress]
        public string Email { get; set; } = default!;

        [Required, MinLength(8)]
        public string Password { get; set; } = default!;

        [Required]
        public string Role { get; set; } = default!; // "SuperAdmin" | "Admin" | "Moderator"
    }
}
