using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.DTOs.Admin.WorkspaceUser
{
    public class SystemStatsDto
    {
        public int TotalWorkspaces { get; set; }
        public int ActiveWorkspaces { get; set; }
        public int PendingWorkspaces { get; set; }
        public int SuspendedWorkspaces { get; set; }
        public int TotalUsers { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalOutstanding { get; set; }
    }
}
