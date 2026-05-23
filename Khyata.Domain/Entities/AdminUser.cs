using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Khyata.Domain.Entities
{
    public class AdminUser : IdentityUser<Guid>
    {
        public string DisplayName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginAt { get; set; }

    }
}
