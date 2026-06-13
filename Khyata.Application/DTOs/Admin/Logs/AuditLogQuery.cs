using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.DTOs.Admin.Logs
{
    public class AuditLogQuery
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
        public Guid? ActorId { get; set; }
        public string? Action { get; set; }
        public string? HttpMethod { get; set; }
        public int? StatusCode { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
