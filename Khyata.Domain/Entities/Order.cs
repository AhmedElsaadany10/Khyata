
using Khyata.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khyata.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }= Guid.NewGuid();
        public Guid WorkspaceId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid CreatedById { get; set; }
        public Guid? UpdatedById { get; set; }
        public string? Description { get; set; }
        public decimal TotalPrice { get; set; }
        public ICollection<OrderPayment> Payments { get; set; } = new List<OrderPayment>();
       
        [NotMapped]
        public decimal TotalPaid => Payments.Sum(p => p.Amount);
        [NotMapped]
        public decimal RemainingAmount => TotalPrice - TotalPaid;
        public OrderStatus Status { get; set; } = OrderStatus.New;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime DeliveryDate { get; set; }
        public string? ExtraNotes { get; set; }

        public Workspace Workspace { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
        public User CreatedBy { get; set; } = null!;
        public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    }
}
