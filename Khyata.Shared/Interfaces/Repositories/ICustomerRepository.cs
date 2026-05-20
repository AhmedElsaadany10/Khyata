using khyata.Application.DTOs.Customer.Requests;
using khyata.Application.DTOs.Customer.Responses;
using Khyata.Application.Common;
using Khyata.Shared.Pagination;

namespace khyata.Application.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<Result<CustomerResponseDto>> CreateAsync(Guid workspaceId, Guid createdBy, CreateCustomerDto dto);
        Task<Result<CustomerResponseDto>> GetByIdAsync(Guid workspaceId, Guid customerId);
        Task<Result<CustomerResponseDto>> UpdateAsync(Guid workspaceId, Guid customerId, Guid updatedBy, UpdateCustomerDto dto);
        Task<Result<CustomerPhoneDto>> AddPhoneAsync(Guid workspaceId, Guid customerId, AddCustomerPhoneDto dto);
        Task<Result> RemovePhoneAsync(Guid workspaceId, Guid customerId, Guid phoneId);
        Task<Result<PagedResult<CustomersListItemDto>>> GetAllAsync(Guid workspaceId, CustomerQuery query);
    }
}
