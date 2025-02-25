using ClubMembership_Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClubMembership_Authentication.Infrastructure.Persistence.Seeders
{
    public class RolesSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
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
        }
    }
}
