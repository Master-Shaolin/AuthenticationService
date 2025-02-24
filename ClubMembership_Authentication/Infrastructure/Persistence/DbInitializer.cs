using ClubMembership_Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ClubMembership_Authentication.Infrastructure.Persistence
{
    public class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            await context.Database.MigrateAsync();

            // Seed roles if they don't exist
            if (!await context.Roles.AnyAsync())
            {
                var roles = new[]
                {
                    new Role { Name = "Admin" },
                    new Role { Name = "User" }
                };

                await context.Roles.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }

            // Get Admin role
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");

            if (adminRole == null)
                throw new Exception("Admin role was not created.");

            // Check if an admin user already exists
            if (!await context.Users.AnyAsync(u => u.Email == "admin@club.com"))
            {
                var adminUser = new User
                {
                    Name = "Super",
                    LastName = "Admin",
                    Email = "admin@club.com",
                    PasswordHash = HashPassword("Admin@123"),
                    RoleId = adminRole.Id,
                    CreatedAt = DateTime.UtcNow
                };

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }
        }

        private static string HashPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
