using ClubMembership_Authentication.Domain.Common;
using ClubMembership_Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClubMembership_Authentication.Infrastructure.Persistence.Seeders
{
    public class UsersSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Get Admin role
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin") ?? throw new Exception("Admin role was not created.");

            // Check if an admin user already exists
            if (!await context.Users.AnyAsync(u => u.Email == "admin@club.com"))
            {
                var adminUser = new User
                {
                    Name = "Super",
                    LastName = "Admin",
                    Email = "admin@club.com",
                    PasswordHash = Utils.HashPassword("Admin@123"),
                    RoleId = adminRole.Id,
                    CreatedAt = DateTime.UtcNow
                };

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}
