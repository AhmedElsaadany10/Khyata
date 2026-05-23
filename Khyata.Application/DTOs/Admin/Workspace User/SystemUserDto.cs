using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.DTOs.Admin.Workspace_User
{
    public class SystemUserDto
    {
        public Guid Id { get; set; }
        public Guid WorkspaceId { get; set; }
        public string WorkspaceName { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Role { get; set; } = default!;
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
