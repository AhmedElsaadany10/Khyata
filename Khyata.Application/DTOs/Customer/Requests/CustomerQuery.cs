namespace Khyata.Application.DTOs.Customer.Requests
{
    public class CustomerQuery
    {
        public string? Search { get; set; }

        public int Page { get; set; } = 1;

        public int Limit { get; set; } = 20;
    }
}
