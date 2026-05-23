using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.DTOs.Admin.Auth
{
    public class AdminLoginRequestDto
    {
        [Required]
        public string Username { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;
    }
}
