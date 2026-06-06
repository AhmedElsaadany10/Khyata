using Khyata.Application.DTOs.Order.Payment;

namespace Khyata.Application.DTOs.Order
{
    public class OrderResponseDto
    {
        public Guid Id { get; set; }

        public Guid WorkspaceId { get; set; }

        public string Description { get; set; } = default!;
        public string? ExtraNotes { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal TotalPaid { get; set; }
        public decimal RemainingAmount { get; set; }
        public string Status { get; set; } = default!;

        public DateTime DeliveryDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
        // Newly added: list of allowed statuses that the order may transition to from current Status
        public IEnumerable<string> AvailableStatuses { get; set; } = Array.Empty<string>();
        public List<OrderPaymentResponseDto> Payments { get; set; } = new();
        public List<OrderStatusHistoryDto> StatusHistory { get; set; } = new();
        public OrderCustomerDto Customer { get; set; } = default!;

        public OrderCreatedByDto CreatedBy { get; set; } = default!;
    }
}
