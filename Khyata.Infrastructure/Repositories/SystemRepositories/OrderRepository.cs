using AutoMapper;
using Khyata.Infrastructure.Data;
using Khyata.Application.DTOs.Order;
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
    public class OrderRepository: IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrderRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<OrderResponseDto>> CreateAsync(Guid workspaceId, Guid createdById, CreateOrderDto dto)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == dto.CustomerId && c.WorkspaceId == workspaceId);
            if (customer is null)
                return Result<OrderResponseDto>.Failure(ApiError.NotFound("Customer not found."));

            // Validate creator belongs to workspace
            var creatorExists = await _context.Users
                .AnyAsync(u => u.Id == createdById && u.WorkspaceId == workspaceId);
            if (!creatorExists)
                return Result<OrderResponseDto>.Failure(ApiError.Forbidden("User does not belong to this workspace."));

            // Validate total price
            if (dto.TotalPrice < 0.01m)
                return Result<OrderResponseDto>.Failure(ApiError.BadRequest("Total price must be greater than zero."));

            // Validate amount paid
            if (dto.AmountPaid < 0 || dto.AmountPaid > dto.TotalPrice)
                return Result<OrderResponseDto>.Failure(
                    ApiError.UnprocessableEntity("Amount paid cannot be negative or exceed the total price."));

            var order = new Order
            {
                WorkspaceId = workspaceId,
                CustomerId = dto.CustomerId,
                CreatedById = createdById,
                Description = dto.Description,
                TotalPrice = dto.TotalPrice,
                ExtraNotes = dto.ExtraNotes,
                DeliveryDate = dto.DeliveryDate,

                Status = OrderStatus.New
            };
            
            _context.Orders.Add(order);

            if (dto.AmountPaid > 0)
            {
                order.Payments.Add(new OrderPayment
                {
                    Amount = dto.AmountPaid,
                    PaymentDate = DateTime.UtcNow,
                    ReceivedById = createdById,
                    Notes = "Initial deposit at order creation",
                    CreatedAt = DateTime.UtcNow
                });
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                var inner = dbEx.InnerException?.Message ?? dbEx.Message;
                return Result<OrderResponseDto>.Failure(
                    ApiError.Internal($"Failed to save order: {inner}"));
            }
            catch (Exception ex)
            {
                return Result<OrderResponseDto>.Failure(
                    ApiError.Internal($"An unexpected error occurred while creating the order: {ex.Message}"));
            }

            return await GetByIdAsync(workspaceId, order.Id);
        }
        public async Task<Result<OrderResponseDto>> GetByIdAsync(Guid workspaceId, Guid orderId)
        {
            var order = await _context.Orders
            .Include(o => o.Customer).ThenInclude(c => c.Phones)
            .Include(o => o.CreatedBy)
            .Include(o => o.Payments.OrderBy(p => p.PaymentDate))
                .ThenInclude(p => p.ReceivedBy)
            .Include(o => o.StatusHistory.OrderBy(h => h.UpdatedAt))
                .ThenInclude(h => h.UpdatedBy)
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(o => o.Id == orderId && o.WorkspaceId == workspaceId);


            if (order is null)
                return Result<OrderResponseDto>.Failure(
                    ApiError.NotFound("Order not found."));

            return Result<OrderResponseDto>.Success(
                _mapper.Map<OrderResponseDto>(order));
        }
        public async Task<Result<PagedResult<OrderListItemDto>>> GetAllAsync(
     Guid workspaceId,
     Guid requestingUserId,
     OrderQuery query)
        {
            var q = _context.Orders
                .Where(o => o.WorkspaceId == workspaceId);

            // Employees can only see their own orders
            if (query.MyOrders)
                q = q.Where(o => o.CreatedById == requestingUserId);

            if (!string.IsNullOrWhiteSpace(query.Status) &&
                Enum.TryParse<OrderStatus>(query.Status, true, out var statusFilter))
                q = q.Where(o => o.Status == statusFilter);

            if (query.CustomerId.HasValue)
                q = q.Where(o => o.CustomerId == query.CustomerId.Value);

            q = q.OrderByDescending(o => o.CreatedAt);

            var result = await PaginationHelper.ToPagedResultAsync(
                q,
                query.Page,
                Math.Clamp(query.Limit, 1, 100),
              o => new OrderListItemDto
              {
                  Id = o.Id,
                  Description = o.Description,

                  TotalPrice = o.TotalPrice,

                  TotalPaid = _context.OrderPayments
        .Where(p => p.OrderId == o.Id)
        .Sum(p => (decimal?)p.Amount) ?? 0,

                  RemainingAmount =
        o.TotalPrice -
        (_context.OrderPayments
            .Where(p => p.OrderId == o.Id)
            .Sum(p => (decimal?)p.Amount) ?? 0),

                  Status = o.Status.ToString(),

                  DeliveredDate = o.DeliveryDate,
                  CreatedBy = _context.Users
                .Where(u => u.Id == o.CreatedById)
                .Select(u => u.Name)
                .FirstOrDefault() ?? "",

                          CustomerName = _context.Customers
                .Where(c => c.Id == o.CustomerId)
                .Select(c => c.Name)
                .FirstOrDefault() ?? "",

                          CustomerPhone = _context.CustomerPhones
                .Where(p => p.CustomerId == o.CustomerId && p.IsPrimary)
                .Select(p => p.Number)
                .FirstOrDefault() ?? ""
                       });

            return Result<PagedResult<OrderListItemDto>>.Success(result);
        }

        public async Task<Result<OrderResponseDto>> UpdateAsync(Guid workspaceId, Guid orderId, Guid updatedBy, string updaterRole, UpdateOrderDto dto)
        {
            // Retrieve the order entity directly from the database, not via GetByIdAsync (which returns Result<OrderResponseDto>)
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.WorkspaceId == workspaceId);
            if (order is null)
                return Result<OrderResponseDto>.Failure(ApiError.NotFound("Order not found."));
            // ── Guard: final status = read-only ──────────────────────────────────
            if (OrderStatusRules.IsFinal(order.Status))
                return Result<OrderResponseDto>.Failure(
                    ApiError.UnprocessableEntity(
                        $"Order is {order.Status} and cannot be modified."));

            // Status transition
            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                if (!Enum.TryParse<OrderStatus>(dto.Status, true, out var newStatus))
                    return Result<OrderResponseDto>.Failure(
                        ApiError.BadRequest($"'{dto.Status}' is not a valid order status."));

                if (!OrderStatusRules.CanTransition(order.Status, newStatus))
                    return Result<OrderResponseDto>.Failure(
                        ApiError.UnprocessableEntity(
                            $"Cannot transition from {order.Status} to {newStatus}."));

                _context.OrderStatusHistories.Add(new OrderStatusHistory
                {
                    OrderId = order.Id,
                    FromStatus = order.Status,
                    ToStatus = newStatus,
                    UpdatedById = updatedBy,
                    UpdatedAt = DateTime.UtcNow
                });

                order.Status = newStatus;
            }
            // ── Price change — Owner only, not after Delivered ────────────────────
            if (dto.TotalPrice.HasValue)
            {
                if (updaterRole != WorkspaceRole.Owner.ToString())
                    return Result<OrderResponseDto>.Failure(
                        ApiError.Forbidden("Only the workspace owner can modify the total price."));

                if (dto.TotalPrice.Value < 0.01m)
                    return Result<OrderResponseDto>.Failure(
                        ApiError.BadRequest("Total price must be greater than zero."));

                var totalPaidAtChange = await _context.OrderPayments
                    .Where(p => p.OrderId == order.Id)
                    .SumAsync(p => p.Amount);


                order.TotalPrice = dto.TotalPrice.Value;
            }
            if (!string.IsNullOrWhiteSpace(dto.Description))
                order.Description = dto.Description;

            if (!string.IsNullOrWhiteSpace(dto.ExtraNotes))
                order.ExtraNotes = dto.ExtraNotes;

            if (dto.TotalPrice.HasValue)
            {
                if (dto.TotalPrice.Value < 0.01m)
                    return Result<OrderResponseDto>.Failure(
                        ApiError.BadRequest("Total price must be greater than zero."));
                order.TotalPrice = dto.TotalPrice.Value;
            }
          
            if (dto.DeliveryDate.HasValue)
                order.DeliveryDate = dto.DeliveryDate.Value;

           // order.UpdatedBy = updatedBy;
            // Update other fields as needed (not shown in original code)
            // Save changes
            await _context.SaveChangesAsync();

            // Return updated order
            return await GetByIdAsync(workspaceId, order.Id);
        }
    }
}
