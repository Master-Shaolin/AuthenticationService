using ClubMembership_Authentication.Infrastructure.Persistence.Seeders;
using Microsoft.EntityFrameworkCore;

namespace ClubMembership_Authentication.Infrastructure.Persistence
{
    public class DbInitializer
    {
        public static async Task Initialize(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
            if (context != null)
            {
                await SeedAsync(context);
            }
        }
        public static async Task SeedAsync(AppDbContext context)
        {
            await context.Database.MigrateAsync();
            await RolesSeeder.SeedAsync(context);
            await UsersSeeder.SeedAsync(context);
        }
    }
}
