using Khyata.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Domain.Entities
{
    public class OrderStatusHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public OrderStatus FromStatus { get; set; }
        public OrderStatus ToStatus { get; set; }

        public Guid UpdatedById { get; set; }
        public User UpdatedBy { get; set; } = null!;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
