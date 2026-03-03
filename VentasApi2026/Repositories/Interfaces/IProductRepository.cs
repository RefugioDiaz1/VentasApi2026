using VentasApi2026.Common;
using VentasApi2026.DTOs;
using VentasApi2026.Models;

namespace VentasApi2026.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> AddAsync(Product product);
        Task<Product?> GetByIdAsync(int id);
        Task<List<Product>> GetByIdsAsync(List<int> ids);  // ✅ nuevo
        Task<PagedResult<Product>> GetPagedAsync(PaginationParams pagination);
        Task SaveChangesAsync();
        Task<bool> DeleteById(int id);
    }
}
