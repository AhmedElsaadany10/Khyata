using System.Diagnostics.Metrics;

namespace khyata.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid WorkspaceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        
        public DateTime? DeletedAt { get; set; }
        public Workspace Workspace { get; set; } = null!;
        public Measurements? Measurements { get; set; }
        public ICollection<CustomerPhone> Phones { get; set; } = [];
        public ICollection<Order> Orders { get; set; } = [];
    }
}
