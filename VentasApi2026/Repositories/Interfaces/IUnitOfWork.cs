using VentasApi2026.Models;
using VentasApi2026.Services.Interfaces;

namespace VentasApi2026.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IProductRepository Products { get; }
        IOrderRepository Orders { get; }
        IUserRepository Users { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        Task SaveChangesAsync();
       
    }
}
