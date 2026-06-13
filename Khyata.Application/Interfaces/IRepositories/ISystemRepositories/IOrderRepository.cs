using Khyata.Application.DTOs.Order;
using Khyata.Application.Common;
using Khyata.Shared.Pagination;

namespace Khyata.Application.Interfaces.IRepositories.ISystemRepositories
{
    public interface IOrderRepository
    {
        Task<Result<OrderResponseDto>> CreateAsync(Guid workspaceId, Guid createdById, CreateOrderDto dto);
        Task<Result<OrderResponseDto>> GetByIdAsync(Guid workspaceId, Guid orderId);
        Task<Result<OrderResponseDto>> UpdateAsync(Guid workspaceId, Guid orderId, Guid updatedBy, string updaterRole, UpdateOrderDto dto);
        Task<Result<PagedResult<OrderListItemDto>>> GetAllAsync(Guid workspaceId, Guid requestingUserId, OrderQuery query);
    }
}
