using AutoMapper;
using Khyata.Application.Common;
using Khyata.Application.DTOs.Order.Payment;
using Khyata.Application.Helpers;
using Khyata.Application.Interfaces.IRepositories.ISystemRepositories;
using Khyata.Domain.Entities;
using Khyata.Domain.Enums;
using Khyata.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Infrastructure.Repositories.SystemRepositories
{
    public class OrderPaymentRepository : IOrderPaymentRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrderPaymentRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<OrderPaymentResponseDto>> AddPaymentAsync(Guid workspaceId, Guid orderId, Guid receivedById, AddPaymentDto dto)
        {
            var order = await _context.Orders
           .Include(o => o.Payments)
           .FirstOrDefaultAsync(o => o.Id == orderId && o.WorkspaceId == workspaceId);

            if (order is null)
                return Result<OrderPaymentResponseDto>.Failure(
                    ApiError.NotFound("Order not found."));

             // Payments can still be added for non-final (e.g. Approved) orders.
        if (OrderStatusRules.IsFinal(order.Status) || order.Status == OrderStatus.Cancelled)
                return Result<OrderPaymentResponseDto>.Failure(
                    ApiError.UnprocessableEntity("Cannot add payment to a cancelled order."));

            if (dto.Amount <= 0)
                return Result<OrderPaymentResponseDto>.Failure(
                    ApiError.BadRequest("Payment amount must be greater than zero."));

            var totalPaid = order.Payments.Sum(p => p.Amount);
            if (totalPaid + dto.Amount > order.TotalPrice)
                return Result<OrderPaymentResponseDto>.Failure(
                    ApiError.UnprocessableEntity(
                        $"Payment would exceed order total. " +
                        $"Remaining: {order.TotalPrice - totalPaid:F2}."));
            var payment = new OrderPayment
            {
                OrderId = orderId,
                Amount = dto.Amount,
                RemainingAfter = order.TotalPrice - (totalPaid + dto.Amount),
                PaymentDate = dto.PaymentDate,
                ReceivedById = receivedById,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.OrderPayments.Add(payment);
            await _context.SaveChangesAsync();

            // Reload with ReceivedBy for the response
            await _context.Entry(payment).Reference(p => p.ReceivedBy).LoadAsync();

            return Result<OrderPaymentResponseDto>.Success(
                _mapper.Map<OrderPaymentResponseDto>(payment));

        }
    }
}
