using System.ComponentModel.DataAnnotations;

namespace Khyata.Application.DTOs.Order
{
    public class CreateOrderDto
    {
        [Required]
        public Guid CustomerId { get; set; }

        [Required, MaxLength(1000)]
        public string Description { get; set; } = default!;
        


        [Required, Range(0.01, double.MaxValue)]
        public decimal TotalPrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal AmountPaid { get; set; }

        [Required]
        public DateTime DeliveryDate { get; set; }
        [MaxLength(1000)]
        public string? ExtraNotes { get; set; }
    }
}
