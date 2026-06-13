using Khyata.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Khyata.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Workspace> Workspaces => Set<Workspace>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<CustomerPhone> CustomerPhones => Set<CustomerPhone>();
        public DbSet<Measurements> Measurements => Set<Measurements>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderStatusHistory> OrderStatusHistories => Set<OrderStatusHistory>();
        public DbSet<OrderPayment> OrderPayments { get; set; }
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ── Global query filters (soft delete) ────────────────────────────────
            // EF Core automatically appends WHERE IsDeleted = 0 to all User and Customer queries.
            // Use .IgnoreQueryFilters() when the admin layer needs to see deleted records.
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<Customer>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Order>().HasQueryFilter(o => !o.Customer.IsDeleted);
            // ── Workspace ─────────────────────────────────────────────────────────
            modelBuilder.Entity<Workspace>(e =>
            {
                e.HasKey(w => w.Id);
                e.Property(w => w.Name).HasMaxLength(200).IsRequired();
                e.Property(w => w.Status).HasConversion<string>().HasMaxLength(30);
                e.HasIndex(w => w.Status);
                e.HasIndex(w => w.NextSuspensionDate);
            });

            // ── User ──────────────────────────────────────────────────────────────
            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(u => u.Id);
                e.HasIndex(u => new { u.WorkspaceId, u.Phone }).IsUnique()
                 .HasFilter("[IsDeleted] = 0");    // allow same phone on soft-deleted records
                e.HasIndex(u => u.Phone);
                e.Property(u => u.Name).HasMaxLength(200).IsRequired();
                e.Property(u => u.Phone).HasMaxLength(20).IsRequired();
                e.Property(u => u.PasswordHash).IsRequired();
                e.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
                e.HasOne(u => u.Workspace)
                 .WithMany(w => w.Users)
                 .HasForeignKey(u => u.WorkspaceId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Customer ──────────────────────────────────────────────────────────
            modelBuilder.Entity<Customer>(e =>
            {
                e.HasKey(c => c.Id);
                e.Property(c => c.Name).HasMaxLength(200).IsRequired();
                e.HasOne(c => c.Workspace)
                 .WithMany(w => w.Customers)
                 .HasForeignKey(c => c.WorkspaceId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(c => c.CreatedBy)
                  .WithMany()
                  .HasForeignKey(c => c.CreatedById)
                  .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(c => c.UpdatedBy)
                    .WithMany()
                    .HasForeignKey(c => c.UpdatedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ── CustomerPhone ─────────────────────────────────────────────────────
            modelBuilder.Entity<CustomerPhone>(e =>
            {
                e.HasKey(p => p.Id);
                // Phone must be unique per workspace (not just per customer)
                e.HasIndex(p => new { p.WorkspaceId, p.Number }).IsUnique();
                e.Property(p => p.Number).HasMaxLength(20).IsRequired();
                e.HasOne(p => p.Customer)
                 .WithMany(c => c.Phones)
                 .HasForeignKey(p => p.CustomerId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Measurements ──────────────────────────────────────────────────────
            modelBuilder.Entity<Measurements>(e =>
            {
                e.HasKey(m => m.Id);
                e.HasIndex(m => m.CustomerId).IsUnique(); // one set per customer
                e.Property(m => m.Height).HasPrecision(6, 2);
                e.Property(m => m.Sleeve).HasPrecision(6, 2);
                e.Property(m => m.ChestWidth).HasPrecision(6, 2);
                e.Property(m => m.Shoulder).HasPrecision(6, 2);
                e.Property(m => m.Neck).HasPrecision(6, 2);
                e.HasOne(m => m.Customer)
                 .WithOne(c => c.Measurements)
                 .HasForeignKey<Measurements>(m => m.CustomerId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Order ─────────────────────────────────────────────────────────────
            modelBuilder.Entity<Order>(e =>
            {
                e.HasKey(o => o.Id);
                e.HasIndex(o => new { o.WorkspaceId, o.Status });
                e.HasIndex(o => new { o.WorkspaceId, o.CustomerId });
                e.HasIndex(o => new { o.WorkspaceId, o.CreatedById });
                e.Property(o => o.Description).HasMaxLength(1000);
                e.Property(o => o.TotalPrice).HasPrecision(12, 2).IsRequired();
               // e.Property(o => o.AmountPaid).HasPrecision(12, 2).IsRequired();
                e.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);
               // e.Ignore(o => o.RemainingBalance); // computed, never stored
                e.HasOne(o => o.Customer)
                 .WithMany(c => c.Orders)
                 .HasForeignKey(o => o.CustomerId)
                 .OnDelete(DeleteBehavior.Restrict)
                 .IsRequired();
                e.HasOne(o => o.CreatedBy)
                 .WithMany(u => u.CreatedOrders)
                 .HasForeignKey(o => o.CreatedById)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(o => o.Workspace)
                 .WithMany(w => w.Orders)
                 .HasForeignKey(o => o.WorkspaceId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ── OrderStatusHistory ────────────────────────────────────────────────────
            modelBuilder.Entity<OrderStatusHistory>(e =>
            {
                e.HasKey(h => h.Id);

                e.HasIndex(h => h.OrderId);
                e.HasIndex(h => h.UpdatedById);
                e.HasIndex(h => h.UpdatedAt);

                e.Property(h => h.FromStatus)
                 .HasConversion<string>()
                 .HasMaxLength(20)
                 .IsRequired();

                e.Property(h => h.ToStatus)
                 .HasConversion<string>()
                 .HasMaxLength(20)
                 .IsRequired();

                e.HasOne(h => h.Order)
                 .WithMany(o => o.StatusHistory)
                 .HasForeignKey(h => h.OrderId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(h => h.UpdatedBy)
                 .WithMany()
                 .HasForeignKey(h => h.UpdatedById)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }

        //public override Task<int> SaveChangesAsync(CancellationToken ct = default)
        //{
        //    foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        //        if (entry.State == EntityState.Modified)
        //            entry.Entity.UpdatedAt = DateTime.UtcNow;

        //    return base.SaveChangesAsync(ct);
        //}
    }
}