using Khyata.Application.DTOs.Customer.Requests;
using Khyata.Application.DTOs.Order;
using Khyata.Domain.Entities;

namespace Khyata.Application.DTOs.Customer.Responses
{
    public class CustomerResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public int OrdersCount { get; set; }
        public string CreatedBy { get; set; } = default!;

        public DateTime CreatedAt { get; set; }

        public MeasurementsDto? Measurements { get; set; }

        public IReadOnlyList<CustomerPhoneDto> Phones { get; set; }
            = new List<CustomerPhoneDto>();

        public IReadOnlyList<CustomerOrderSummaryDto> Orders { get; set; }
            = new List<CustomerOrderSummaryDto>();
    }
}
