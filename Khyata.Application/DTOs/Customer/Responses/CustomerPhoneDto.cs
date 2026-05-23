namespace Khyata.Application.DTOs.Customer.Responses
{
    public class CustomerPhoneDto
    {
        public Guid Id { get; set; }

        public string Number { get; set; } = default!;

        public bool IsPrimary { get; set; }
    }
}