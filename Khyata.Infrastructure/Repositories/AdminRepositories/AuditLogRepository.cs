using AutoMapper;
using Khyata.Application.Common;
using Khyata.Application.DTOs.Admin.Logs;
using Khyata.Application.Interfaces.IRepositories.IAdminRepositories;
using Khyata.Domain.Entities;
using Khyata.Infrastructure.Data;
using Khyata.Infrastructure.Helpers;
using Khyata.Shared.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Infrastructure.Repositories.AdminRepositories
{
    internal class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AuditLogRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<AuditLogResponseDto>>> GetAllAsync(AuditLogQuery query)
        {
            var q = _context.AuditLogs.AsQueryable();

            if (query.ActorId.HasValue)
                q = q.Where(a => a.ActorId == query.ActorId.Value);

            if (!string.IsNullOrWhiteSpace(query.Action))
                q = q.Where(a => a.Action.Contains(query.Action));

            if (!string.IsNullOrWhiteSpace(query.HttpMethod))
                q = q.Where(a => a.HttpMethod == query.HttpMethod.ToUpper());

            if (query.StatusCode.HasValue)
                q = q.Where(a => a.StatusCode == query.StatusCode.Value);

            if (query.From.HasValue)
                q = q.Where(a => a.Timestamp >= query.From.Value);

            if (query.To.HasValue)
                q = q.Where(a => a.Timestamp <= query.To.Value);

            q = q.OrderByDescending(a => a.Timestamp);

            var result = await PaginationHelper.ToPagedResultAsync(
                q,
                query.Page,
                Math.Clamp(query.Limit, 1, 100),
                a => _mapper.Map<AuditLogResponseDto>(a));

            return Result<PagedResult<AuditLogResponseDto>>.Success(result);
        }

        public async Task<Result<AuditLogResponseDto>> GetByIdAsync(Guid id)
        {
            var log = await _context.AuditLogs.FindAsync(id);

            if (log is null)
                return Result<AuditLogResponseDto>.Failure(ApiError.NotFound("Audit log not found."));

            return Result<AuditLogResponseDto>.Success(_mapper.Map<AuditLogResponseDto>(log));
        }

        public async Task LogAsync(AuditLog entry)
        {
            _context.AuditLogs.Add(entry);
            await _context.SaveChangesAsync();
        }
    }
}
