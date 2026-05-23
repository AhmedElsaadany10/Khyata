using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.DTOs.Admin.AdminUser
{
    public class ChangeAdminPasswordDto
    {
        [Required, MinLength(8)]
        public string CurrentPassword { get; set; } = default!;

        [Required, MinLength(8)]
        public string NewPassword { get; set; } = default!;
    }
}
