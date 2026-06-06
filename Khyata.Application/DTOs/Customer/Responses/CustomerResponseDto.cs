using Khyata.Application.DTOs.Customer.Requests;

namespace Khyata.Application.DTOs.Customer.Responses
{
    public class CustomerResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;

        public DateTime CreatedAt { get; set; }

        public MeasurementsDto? Measurements { get; set; }

        public IReadOnlyList<CustomerPhoneDto> Phones { get; set; }
            = new List<CustomerPhoneDto>();
    }
}
