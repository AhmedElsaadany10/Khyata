using Khyata.Application.DTOs.Admin.Logs;
using Khyata.Application.Extensions;
using Khyata.Application.Interfaces.IRepositories.IAdminRepositories;
using Khyata.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Khyata.Admin.Controllers
{
    [ApiController]
    [Route("admin/audit-logs")]
     [Authorize(Policy = AdminPolicies.AnyAdmin)]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogRepository _auditRepo;

        public AuditLogsController(IAuditLogRepository auditRepo)
            => _auditRepo = auditRepo;

        // GET /api/audit-logs
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] AuditLogQuery query)
        {
            var result = await _auditRepo.GetAllAsync(query);
            return this.ToActionResult(result);

        }

        // GET /api/audit-logs/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _auditRepo.GetByIdAsync(id);
            return this.ToActionResult(result);

        }
    }
}
