using khyata.Domain.Enums;

namespace khyata.Domain.Entities
{
    public class Workspace
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public WorkspaceStatus Status { get; set; } = WorkspaceStatus.PendingActivation;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>UTC timestamp of the last activation. Used by the suspension background service.</summary>
        public DateTime? LastActivatedAt { get; set; }

        /// <summary>Next scheduled suspension date (set when workspace is activated).</summary>
        public DateTime? NextSuspensionDate { get; set; }

        public ICollection<User> Users { get; set; } = [];
        public ICollection<Customer> Customers { get; set; } = [];
        public ICollection<Order> Orders { get; set; } = [];
    }
}
