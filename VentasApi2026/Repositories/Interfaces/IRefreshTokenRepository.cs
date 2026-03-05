using VentasApi2026.Models;

namespace VentasApi2026.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<RefreshToken?> GetActiveTokenAsync(string token);
    }
}
