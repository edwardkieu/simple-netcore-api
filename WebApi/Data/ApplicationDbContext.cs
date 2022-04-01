using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Data.Common;
using WebApi.Data.Entities;

namespace WebApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<AppUser> AppUsers { get; set; }

        public DbSet<TestTable> TestTables { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>(entity =>
            {
                entity.ToTable(name: "AppUsers");
            });

            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "AppRoles");
            });

            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("AppUserRoles");
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("AppUserClaims");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("AppUserLogins");
            });

            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("AppRoleClaims");
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("AppUserTokens");
            });

            builder.Entity<TestTable>(entity =>
            {
                entity.ToTable("TestTable");
                entity.HasOne(x => x.User)
                .WithOne(x => x.TestTable)
                .HasForeignKey<TestTable>(x => x.UserId);
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            ApplyAuditEntity();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            ApplyAuditEntity();
            return base.SaveChanges();
        }

        private void ApplyAuditEntity()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("uid");
            if (!string.IsNullOrWhiteSpace(userId))
            {
                foreach (var entry in ChangeTracker.Entries<AuditableEntity>().Where(x => x.State == EntityState.Added || x.State == EntityState.Modified))
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entry.Entity.Created = DateTime.Now;
                            entry.Entity.CreatedBy = userId;
                            break;

                        case EntityState.Modified:
                            entry.Entity.LastModified = DateTime.Now;
                            entry.Entity.LastModifiedBy = userId;
                            break;
                    }
                }
            }
        }
    }
}