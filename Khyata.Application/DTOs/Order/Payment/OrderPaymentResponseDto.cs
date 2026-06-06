using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Application.DTOs.Order.Payment
{
    public class OrderPaymentResponseDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string ReceivedBy { get; set; } = default!;
        public DateTime PaymentDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
