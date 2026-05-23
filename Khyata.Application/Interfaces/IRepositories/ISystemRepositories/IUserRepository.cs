using Khyata.Application.DTOs.Auth;
using Khyata.Application.DTOs.Employee;
using Khyata.Domain.Entities;
using Khyata.Application.Common;
using Khyata.Shared.Pagination;
namespace Khyata.Application.Interfaces.IRepositories.ISystemRepositories
{
    public interface IUserRepository
    {
        Task<Result<UserResponseDto>> CreateEmployeeAsync(Guid workspaceId, CreateEmployeeDto dto);
        Task<Result<EmployeeResponseDto>> GetEmployeeByIdAsync(Guid workspaceId, Guid userId);
        Task<Result<PagedResult<EmployeeResponseDto>>> GetEmployeesAsync(Guid workspaceId, PaginationQuery query);
        Task<Result<EmployeeResponseDto>> UpdateEmployeeAsync(Guid workspaceId, Guid userId, Guid ownerId, UpdateEmployeeDto dto);
        Task<Result<EmployeeResponseDto>> UpdateOwnerAsync(Guid userId, UpdateEmployeeDto dto);
        Task<Result> DeleteEmployeeAsync(Guid workspaceId, Guid employeeId, Guid ownerId);

        // Used publicly
        Task<User?> FindByPhoneAsync(string phone);
        Task<bool> PhoneExistsInWorkspaceAsync(Guid workspaceId, string phone, Guid? excludeUserId = null);
    }
}
