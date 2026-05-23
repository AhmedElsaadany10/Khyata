namespace Khyata.Application.DTOs.Customer.Responses
{
    public class CustomersListItemDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string PrimaryPhone { get; set; } = default!;

    }
}
