using System.ComponentModel.DataAnnotations;

namespace khyata.Application.DTOs.Customer.Requests
{
    public class UpdateCustomerDto
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        public MeasurementsDto? Measurements { get; set; }
    }
}
