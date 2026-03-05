namespace VentasApi2026.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;       

        public int TokenVersion { get; set; } = 1; // ✅ nuevo
        public int? CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<RefreshToken> RefreshTokens { get; set; }
        = new List<RefreshToken>();

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>(); // ✅
}
}
