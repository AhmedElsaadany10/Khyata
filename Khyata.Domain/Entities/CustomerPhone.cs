namespace Khyata.Domain.Entities
{
    public class CustomerPhone
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid WorkspaceId { get; set; }
        public string Number { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }

        public Customer Customer { get; set; } = null!;
    }
}