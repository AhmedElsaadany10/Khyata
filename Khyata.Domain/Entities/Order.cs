
using Khyata.Domain.Enums;

namespace Khyata.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid WorkspaceId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
        public string? Description { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal RemainingBalance => TotalPrice - AmountPaid;
        public OrderStatus Status { get; set; } = OrderStatus.New;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime DeliveryDate { get; set; }
        public string? ExtraNotes { get; set; }

        public Workspace Workspace { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
        public User CreatedBy { get; set; } = null!;
    }
}
