namespace Khyata.Application.DTOs.Order
{
    public class OrderListItemDto
    {
        public Guid Id { get; set; }

        public string? Description { get; set; }

        public decimal TotalPrice { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal RemainingAmount { get; set; }

        public string Status { get; set; } = default!;

        public DateTime DeliveredDate { get; set; }
        public string CreatedBy { get; set; } = default!;

        public string CustomerName { get; set; } = default!;
        public string CustomerPhone { get; set; } = default!;
    }
}