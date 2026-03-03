using VentasApi2026.Common;
using VentasApi2026.DTOs;

namespace VentasApi2026.Services.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<PagedResult<ProductDto>> GetPagedAsync(PaginationParams pagination);
        Task<ProductDto> getByIdAsync(int id);
        Task<ProductDto> UpdateProduct(int id, UpdateProductDto data);
        Task<bool> DeleteById(int id);
    }
}
