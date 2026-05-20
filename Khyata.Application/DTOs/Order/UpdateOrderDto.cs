using System.ComponentModel.DataAnnotations;

namespace khyata.Application.DTOs.Order
{
    public class UpdateOrderDto
    {
        [MaxLength(1000)]
        public string? Description { get; set; }
        public string? ExtraNotes { get; set; }

        public decimal? TotalPrice { get; set; }

        public decimal? AmountPaid { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public string? Status { get; set; }
    }
}
