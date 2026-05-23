using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.DTOs.Admin.Logs
{
    public class AuditLogDto
    {
        public Guid Id { get; set; }
        public Guid? ActorId { get; set; }
        public string? ActorName { get; set; }
        public string Action { get; set; } = default!;
        public string EntityType { get; set; } = default!;
        public Guid? EntityId { get; set; }
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
