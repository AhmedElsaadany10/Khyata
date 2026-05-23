using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.DTOs.Admin.Role
{
    public class AssignRoleDto
    {
        [Required]
        public string Role { get; set; } = default!;
    }
}
