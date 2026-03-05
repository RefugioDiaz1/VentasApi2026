using Microsoft.EntityFrameworkCore;
using VentasApi2026.Data;
using VentasApi2026.Models;
using VentasApi2026.Repositories.Interfaces;

namespace VentasApi2026.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context) => _context = context;

        public async Task AddAsync(RefreshToken token)
            => await _context.RefreshTokens.AddAsync(token);

        public async Task<RefreshToken?> GetActiveTokenAsync(string token)
            => await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r =>
                    r.Token == token &&
                    !r.IsRevoked &&
                    r.ExpiresAt > DateTime.UtcNow);
    }
}
