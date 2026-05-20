namespace khyata.Application.DTOs.Order
{
    public class OrderCustomerDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string PrimaryPhone { get; set; } = default!;
    }
}
