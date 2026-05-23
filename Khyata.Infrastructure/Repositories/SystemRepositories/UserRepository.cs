using AutoMapper;
using Khyata.Infrastructure.Data;
using Khyata.Application.DTOs.Auth;
using Khyata.Application.DTOs.Employee;
using Khyata.Domain.Enums;
using Khyata.Application.Helpers;
using Khyata.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Khyata.Shared.Pagination;
using Khyata.Application.Common;
using Khyata.Infrastructure.Helpers;
using Khyata.Application.Interfaces.IRepositories.ISystemRepositories;

namespace Khyata.Infrastructure.Repositories.SystemRepositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<Result<UserResponseDto>> CreateEmployeeAsync(Guid workspaceId, CreateEmployeeDto dto)
        {
            // Try to find any user record (including soft-deleted) with the same phone in the workspace
            var existing = await _context.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.WorkspaceId == workspaceId && u.Phone == dto.Phone);
            if (existing is not null)
            {
                if (!existing.IsDeleted)
                {
                    // Active user exists — conflict
                    return Result<UserResponseDto>.Failure(
                        ApiError.Conflict("An employee with this phone number already exists in your workspace."));
                }

                // Reactivate the soft-deleted user instead of creating a new row
                existing.IsDeleted = false;
                existing.DeletedAt = null;
                existing.Name = dto.Name;
                existing.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                existing.Role = WorkspaceRole.Employee;
                existing.CreatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Load workspace navigation property
                await _context.Entry(existing)
                    .Reference(u => u.Workspace)
                    .LoadAsync();

                return Result<UserResponseDto>.Success(_mapper.Map<UserResponseDto>(existing));
            }

            // No existing user — create a new one
            var employee = new User
            {
                WorkspaceId = workspaceId,
                Name = dto.Name,
                Phone = dto.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = WorkspaceRole.Employee
            };

            _context.Users.Add(employee);
            await _context.SaveChangesAsync();
            // Load workspace navigation property
            await _context.Entry(employee)
                .Reference(u => u.Workspace)
                .LoadAsync();

            return Result<UserResponseDto>.Success(_mapper.Map<UserResponseDto>(employee));
        }


        public async Task<User?> FindByPhoneAsync(string phone)
        {
            return await _context.Users.IgnoreQueryFilters()
            .Include(u => u.Workspace)
            .FirstOrDefaultAsync(u => u.Phone == phone);
        }
        public async Task<bool> PhoneExistsInWorkspaceAsync(Guid workspaceId, string phone, Guid? excludeUserId = null)
        {
            var result = _context.Users.Where(u => u.WorkspaceId == workspaceId && u.Phone == phone);
            if (excludeUserId.HasValue) 
                result = result.Where(u => u.Id != excludeUserId.Value);
            return await result.AnyAsync();
        }

        public async Task<Result<EmployeeResponseDto>> GetEmployeeByIdAsync(Guid workspaceId, Guid userId)
        {
            var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId && u.WorkspaceId == workspaceId && u.Role == WorkspaceRole.Employee);

            return user is null
                ? Result<EmployeeResponseDto>.Failure(ApiError.NotFound("Employee not found."))
                : Result<EmployeeResponseDto>.Success(_mapper.Map<EmployeeResponseDto>(user));
        }

        public async Task<Result<PagedResult<EmployeeResponseDto>>> GetEmployeesAsync(Guid workspaceId, PaginationQuery query)
        {
            var q = _context.Users
           .Where(u => u.WorkspaceId == workspaceId && u.Role == WorkspaceRole.Employee)
           .OrderBy(u => u.Name);

            var result = await PaginationHelper.ToPagedResultAsync(q, query.SafePage, query.SafeLimit,
                u => _mapper.Map<EmployeeResponseDto>(u));

            return Result<PagedResult<EmployeeResponseDto>>.Success(result);
        }
        public async Task<Result<EmployeeResponseDto>> UpdateEmployeeAsync(Guid workspaceId, Guid userId,Guid ownerId, UpdateEmployeeDto dto)
        {
            // Verify updater is the Owner in the workspace
            var owner = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == ownerId && u.WorkspaceId == workspaceId && u.Role == WorkspaceRole.Owner);

            if (owner is null)
                return Result<EmployeeResponseDto>.Failure(ApiError.Forbidden("Only owner can update employees."));

            var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId && u.WorkspaceId == workspaceId && u.Role == WorkspaceRole.Employee);

            if (user is null)
                return Result<EmployeeResponseDto>.Failure(ApiError.NotFound("Employee not found."));

            return await ApplyUserUpdate(user, dto);
        }

        public async Task<Result<EmployeeResponseDto>> UpdateOwnerAsync(Guid userId, UpdateEmployeeDto dto)
        {
            var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId && u.Role == WorkspaceRole.Owner);

            if (user is null)
                return Result<EmployeeResponseDto>.Failure(ApiError.NotFound("Owner not found."));

            return await ApplyUserUpdate(user, dto);
        }
        public async Task<Result> DeleteEmployeeAsync(Guid workspaceId, Guid employeeId, Guid ownerId)
        {
            var owner = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == ownerId && u.WorkspaceId == workspaceId && u.Role == WorkspaceRole.Owner);

            if (owner is null)
                return Result.Failure(ApiError.Forbidden("Only owner can delete employees."));

            var employee = await _context.Users
           .FirstOrDefaultAsync(u =>
               u.Id == employeeId &&
               u.WorkspaceId == workspaceId &&
               u.Role == WorkspaceRole.Employee);

            if (employee is null)
                return Result.Failure(ApiError.NotFound("Employee not found."));

            employee.IsDeleted = true;
            employee.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Result.Success();
        }
        private async Task<Result<EmployeeResponseDto>> ApplyUserUpdate(User user, UpdateEmployeeDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.Name))
                user.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                // Require current password verification when updating own password
                if (!string.IsNullOrWhiteSpace(dto.CurrentPassword) &&
                    !BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                {
                    return Result<EmployeeResponseDto>.Failure(
                        ApiError.BadRequest("Current password is incorrect."));
                }
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            }

            await _context.SaveChangesAsync();
            return Result<EmployeeResponseDto>.Success(_mapper.Map<EmployeeResponseDto>(user));
        }


    }
}
