using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.DTOs.Admin.WorkspaceUser
{
    public class WorkspaceSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string OwnerName { get; set; } = default!;
        public string OwnerPhone { get; set; } = default!;
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalEmployees { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? NextSuspensionDate { get; set; }
    }
}
