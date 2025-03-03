using ClubMembership_Authentication.Domain.Entities;
using ClubMembership_Authentication.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClubMembership_Authentication.Infrastructure.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task<Role?> GetUserRoleAsync(string roleName)
        {
            return await context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        }

        public async Task<bool> ExistsAsync(Guid userId)
        {
            return await context.Users.AnyAsync(u => u.Id == userId);
        }
    }
}
