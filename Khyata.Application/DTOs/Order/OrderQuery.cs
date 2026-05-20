namespace khyata.Application.DTOs.Order
{
    public class OrderQuery
    {
        public string? Status { get; set; }

        public Guid? CustomerId { get; set; }

        // filter to orders created by the requesting user
        public bool MyOrders { get; set; } = false;

        public int Page { get; set; } = 1;

        public int Limit { get; set; } = 20;
    }
}
