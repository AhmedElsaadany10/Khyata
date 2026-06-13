using AutoMapper;
using Khyata.Application.Common;
using Khyata.Application.DTOs.Admin.Logs;
using Khyata.Application.DTOs.Admin.Workspace_User;
using Khyata.Application.DTOs.Admin.WorkspaceUser;
using Khyata.Application.Helpers;
using Khyata.Application.Interfaces.IRepositories.IAdminRepositories;
using Khyata.Domain.Entities;
using Khyata.Domain.Enums;
using Khyata.Infrastructure.Data;
using Khyata.Infrastructure.Helpers;
using Khyata.Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Infrastructure.Repositories.AdminRepositories
{
    internal class AdminWorkspaceRepository : IAdminWorkspaceRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AdminWorkspaceRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<Result<PagedResult<WorkspaceSummaryDto>>> GetWorkspacesAsync(string? status, PaginationQuery query)
        {
            var q = _context.Workspaces
            .Include(w => w.Users)
            .Include(w => w.Orders)
            .Include(w => w.Customers)
            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<WorkspaceStatus>(status, true, out var ws))
                q = q.Where(w => w.Status == ws);

            q = q.OrderByDescending(w => w.CreatedAt);

            var result = await PaginationHelper.ToPagedResultAsync(
                q, query.SafePage, query.SafeLimit,
                w => _mapper.Map<WorkspaceSummaryDto>(w));

            return Result<PagedResult<WorkspaceSummaryDto>>.Success(result);
        }
        public async Task<Result<WorkspaceSummaryDto>> GetWorkspaceAsync(Guid id)
        {
            var ws = await _context.Workspaces
            .Include(w => w.Users)
            .Include(w => w.Orders)
            .Include(w => w.Customers)
            .FirstOrDefaultAsync(w => w.Id == id);

                    return ws is null
                        ? Result<WorkspaceSummaryDto>.Failure(ApiError.NotFound("Workspace not found."))
                        : Result<WorkspaceSummaryDto>.Success(_mapper.Map<WorkspaceSummaryDto>(ws));
        }

        public async Task<Result<WorkspaceSummaryDto>> UpdateWorkspaceStatusAsync(
            Guid id, UpdateWorkspaceStatusDto dto)
        {
            var ws = await _context.Workspaces.FindAsync(id);
            if (ws is null)
                return Result<WorkspaceSummaryDto>.Failure(ApiError.NotFound("Workspace not found."));

            if (!Enum.TryParse<WorkspaceStatus>(dto.Status, true, out var newStatus))
                return Result<WorkspaceSummaryDto>.Failure(
                    ApiError.BadRequest($"'{dto.Status}' is not a valid workspace status."));

            ws.Status = newStatus;

            if (newStatus == WorkspaceStatus.Active)
            {
                ws.LastActivatedAt = DateTime.UtcNow;
                ws.NextSuspensionDate = DateHelper.GetEndOfMonth(DateTime.UtcNow);
            }

            await _context.SaveChangesAsync();

          

            return await GetWorkspaceAsync(id);
        }

        public async Task<Result<SystemUserDto>> GetUserAsync(Guid userId)
        {
            var user = await _context.Users
            .IgnoreQueryFilters()
            .Include(u => u.Workspace)
            .FirstOrDefaultAsync(u => u.Id == userId);

            return user is null
                ? Result<SystemUserDto>.Failure(ApiError.NotFound("User not found."))
                : Result<SystemUserDto>.Success(_mapper.Map<SystemUserDto>(user));
        }
        public async Task<Result<PagedResult<SystemUserDto>>> GetAllUsersAsync(Guid? workspaceId, string? role, bool includeDeleted, PaginationQuery query)
        {
            var q = _context.Users
            .IgnoreQueryFilters()
            .Include(u => u.Workspace)
            .AsQueryable();

            if (!includeDeleted) q = q.Where(u => !u.IsDeleted);
            if (workspaceId.HasValue) q = q.Where(u => u.WorkspaceId == workspaceId.Value);

            if (!string.IsNullOrWhiteSpace(role) &&
                Enum.TryParse<WorkspaceRole>(role, true, out var r))
                q = q.Where(u => u.Role == r);

            q = q.OrderByDescending(u => u.CreatedAt);

            var result = await PaginationHelper.ToPagedResultAsync(
                q, query.SafePage, query.SafeLimit,
                u => _mapper.Map<SystemUserDto>(u));

            return Result<PagedResult<SystemUserDto>>.Success(result);
        }

        public async Task<Result> ResetWorkspaceUserPasswordAsync(Guid userId, ResetWorkspaceUserPasswordDto dto)
        {
            var user = await _context.Users.IgnoreQueryFilters()
           .FirstOrDefaultAsync(u => u.Id == userId);

            if (user is null) return Result.Failure(ApiError.NotFound("User not found."));

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();
          

            return Result.Success();
        }
        public async Task<Result<SystemStatsDto>> GetSystemStatsAsync()
        {
            var totalRevenue = await _context.Orders
                .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;

            var totalPaid = await _context.OrderPayments
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            var stats = new SystemStatsDto
            {
                TotalWorkspaces = await _context.Workspaces.CountAsync(),

                ActiveWorkspaces = await _context.Workspaces
                    .CountAsync(w => w.Status == WorkspaceStatus.Active),

                PendingWorkspaces = await _context.Workspaces
                    .CountAsync(w => w.Status == WorkspaceStatus.PendingActivation),

                SuspendedWorkspaces = await _context.Workspaces
                    .CountAsync(w => w.Status == WorkspaceStatus.Suspended),

                TotalUsers = await _context.Users
                    .IgnoreQueryFilters()
                    .CountAsync(),

                TotalCustomers = await _context.Customers
                    .IgnoreQueryFilters()
                    .CountAsync(),

                TotalOrders = await _context.Orders.CountAsync(),

                TotalRevenue = totalRevenue,

                TotalPaid = totalPaid,

                TotalOutstanding = totalRevenue - totalPaid
            };

            return Result<SystemStatsDto>.Success(stats);
        }
      
    }
}
