using System.ComponentModel.DataAnnotations;

namespace Khyata.Application.DTOs.Customer.Requests
{
    public class CreateCustomerDto
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = default!;

        [Required, MaxLength(20)]
        public string? Address { get; set; }
        public string PrimaryPhone { get; set; } = default!;

        public MeasurementsDto? Measurements { get; set; }
    }
}
