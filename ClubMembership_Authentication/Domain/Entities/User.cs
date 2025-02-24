namespace ClubMembership_Authentication.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key for Role
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}
