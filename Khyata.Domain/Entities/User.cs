using khyata.Domain.Enums;

namespace khyata.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid WorkspaceId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public Workspace Workspace { get; set; } = null!;
        public ICollection<Order> CreatedOrders { get; set; } = [];
    }
}
