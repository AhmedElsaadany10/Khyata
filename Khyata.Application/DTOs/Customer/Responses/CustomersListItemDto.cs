namespace Khyata.Application.DTOs.Customer.Responses
{
    public class CustomersListItemDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;

        public string PrimaryPhone { get; set; } = default!;
        public int OrdersCount { get; set; }
    }
}
