using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.DTOs.Admin.Logs
{
    public class AuditLogResponseDto
    {
        public Guid Id { get; set; }
        public Guid? ActorId { get; set; }
        public string? ActorName { get; set; }
        public string Action { get; set; } = null!;
        public string? RequestBody { get; set; }
        public string? ResponseBody { get; set; }
        public string HttpMethod { get; set; } = null!;
        public string Endpoint { get; set; } = null!;
        public int StatusCode { get; set; }
        public long ExecutionTimeMs { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
