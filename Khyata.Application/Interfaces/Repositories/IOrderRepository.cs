using khyata.Application.DTOs.Order;
using Khyata.Application.Common;
using Khyata.Shared.Pagination;

namespace khyata.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<Result<OrderResponseDto>> CreateAsync(Guid workspaceId, Guid createdById, CreateOrderDto dto);
        Task<Result<OrderResponseDto>> GetByIdAsync(Guid workspaceId, Guid orderId);
        Task<Result<OrderResponseDto>> UpdateAsync(Guid workspaceId, Guid orderId, Guid updatedBy, UpdateOrderDto dto);
        Task<Result<PagedResult<OrderResponseDto>>> GetAllAsync(Guid workspaceId, Guid requestingUserId, OrderQuery query);
    }
}
