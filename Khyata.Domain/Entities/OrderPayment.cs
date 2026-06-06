using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Domain.Entities
{
    public class OrderPayment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public decimal Amount { get; set; }
        public decimal RemainingAfter { get; set; }  

        public DateTime PaymentDate { get; set; }

        public Guid ReceivedById { get; set; }
        public User ReceivedBy { get; set; } = null!;

        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
