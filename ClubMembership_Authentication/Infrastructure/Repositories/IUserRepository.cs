using ClubMembership_Authentication.Domain.Entities;

namespace ClubMembership_Authentication.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task<Role?> GetUserRoleAsync(string roleName);
        Task<bool> ExistsAsync(Guid userId);
    }
}
