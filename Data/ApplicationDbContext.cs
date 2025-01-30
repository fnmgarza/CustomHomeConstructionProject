using CustomHomeConstructionProjects.Models;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CustomHomeConstructionProjects.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the relationship between Project and ApplicationUser
            modelBuilder.Entity<Project>()
                .HasOne(p => p.User) // Navigation property
                .WithMany(u => u.Projects) // A user can have multiple projects
                .HasForeignKey(p => p.UserId) // Foreign key in Project
                .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete projects when a user is deleted

            modelBuilder.Entity<Client>()
                .HasMany(c => c.ClientContacts)
                .WithOne(c => c.Client)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectStatus>().HasData(
                new ProjectStatus { Id = 1, StatusName = "Not Started", ColorHex = "#f39c12" },
                new ProjectStatus { Id = 2, StatusName = "In Progress", ColorHex = "#3498db" },
                new ProjectStatus { Id = 3, StatusName = "Complete", ColorHex = "#2ecc71" }
            );

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" }
            );

            var adminUserId = "3c3b7e4b-6c3a-4f0f-8b16-222f6a1c85b3"; // Static ID for admin user

            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = adminUserId,
                    UserName = "admin@example.com",
                    NormalizedUserName = "ADMIN@EXAMPLE.COM",
                    Email = "admin@example.com",
                    NormalizedEmail = "ADMIN@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null, "Admin@1234"),
                    SecurityStamp = string.Empty,
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }
            );
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientContact> ClientContacts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<ProjectStatus> ProjectStatuses { get; set; }
        public DbSet<ProjectNote> ProjectNotes { get; set; }
    }


}