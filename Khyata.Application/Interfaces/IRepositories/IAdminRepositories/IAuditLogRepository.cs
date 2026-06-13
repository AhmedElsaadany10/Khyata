using Khyata.Application.Common;
using Khyata.Application.DTOs.Admin.Logs;
using Khyata.Domain.Entities;
using Khyata.Shared.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.Interfaces.IRepositories.IAdminRepositories
{
    public interface IAuditLogRepository
    {
        Task LogAsync(AuditLog entry);
        Task<Result<PagedResult<AuditLogResponseDto>>> GetAllAsync(AuditLogQuery query);
        Task<Result<AuditLogResponseDto>> GetByIdAsync(Guid id);
    }
}
