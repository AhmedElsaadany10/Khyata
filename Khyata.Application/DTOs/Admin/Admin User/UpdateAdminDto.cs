using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.DTOs.Admin.AdminUser
{
    public class UpdateAdminDto
    {
        [MaxLength(200)]
        public string? DisplayName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        //public bool? IsActive { get; set; }
    }
}
