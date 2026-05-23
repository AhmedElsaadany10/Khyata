using Khyata.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Khyata.Infrastructure.Data
{
    public class AdminDbContext : IdentityDbContext<AdminUser, IdentityRole<Guid>, Guid>
    {
        public AdminDbContext(DbContextOptions<AdminDbContext> options)
            : base(options)
        {
        }
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("admin");

            builder.Entity<AdminUser>().ToTable("Users");
            builder.Entity<IdentityRole<Guid>>().ToTable("Roles");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

            // ── AdminUser ─────────────────────────────────────────────────────────
            builder.Entity<AdminUser>(e =>
            {
                e.Property(u => u.DisplayName)
                    .HasMaxLength(200)
                    .IsRequired();

            });


            // ── AuditLog ──────────────────────────────────────────────────────────
            builder.Entity<AuditLog>(e =>
            {
                e.HasKey(a => a.Id);
                e.HasIndex(a => new { a.EntityType, a.EntityId });
                e.HasIndex(a => a.Timestamp);
                e.HasIndex(a => a.ActorId);
                e.Property(a => a.Action).HasMaxLength(100).IsRequired();
                e.Property(a => a.EntityType).HasMaxLength(100).IsRequired();
            });
        }
    }
}
