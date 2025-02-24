namespace ClubMembership_Authentication.Events
{
    public class MembershipCreatedEvent
    {
        public Guid UserId { get; set; }
        public string MembershipType { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationInMonths { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
