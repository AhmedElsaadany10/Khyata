namespace khyata.Application.DTOs.Order
{
    public class OrderCreatedByDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string Role { get; set; } = default!;
    }
}
