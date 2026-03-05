using VentasApi2026.Services.Interfaces;

namespace VentasApi2026.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string password, string passordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passordHash);
        }
    }
}
