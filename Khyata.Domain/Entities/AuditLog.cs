using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Domain.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? ActorId { get; set; }
        public string? ActorName { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public Guid? EntityId { get; set; }
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
