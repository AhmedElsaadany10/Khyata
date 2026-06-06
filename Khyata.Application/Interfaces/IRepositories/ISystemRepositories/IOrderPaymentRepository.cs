using Khyata.Application.Common;
using Khyata.Application.DTOs.Order.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.Interfaces.IRepositories.ISystemRepositories
{
    public interface IOrderPaymentRepository
    {
        Task<Result<OrderPaymentResponseDto>> AddPaymentAsync(
        Guid workspaceId, Guid orderId, Guid receivedById, AddPaymentDto dto);

    }
}
