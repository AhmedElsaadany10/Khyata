using System.ComponentModel.DataAnnotations;

namespace Khyata.Application.DTOs.Customer.Requests
{
    public class UpdateCustomerDto
    {
        [MaxLength(200)]
        public string? Name { get; set; }
        [MaxLength(500)]
        public string? Address { get; set; } = null;

        public MeasurementsDto? Measurements { get; set; }
    }
}
