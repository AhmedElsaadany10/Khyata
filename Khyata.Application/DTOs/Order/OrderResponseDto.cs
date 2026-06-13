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

        public string CreatedBy { get; set; } = default!;

        public string CustomerName { get; set; } = default!;
        public string CustomerPhone { get; set; } = default!;

        public IEnumerable<string> AvailableStatuses { get; set; } = Array.Empty<string>();

        public List<OrderPaymentResponseDto> Payments { get; set; } = new();

        public List<OrderStatusHistoryDto> StatusHistory { get; set; } = new();
    }
}
