using Microsoft.EntityFrameworkCore;
using VentasApi2026.Common;
using VentasApi2026.Data;
using VentasApi2026.DTOs;
using VentasApi2026.Models;
using VentasApi2026.Repositories.Interfaces;

namespace VentasApi2026.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) {
            this._context = context;
        }

        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<List<Product>> GetByIdsAsync(List<int> ids)
        {
            return await _context.Products
                .Where(p => ids.Contains(p.Id)) // si tienes soft delete
                .ToListAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteById(int id)
        {
            var rowsAffected = await _context.Products.Where(w => w.Id == id).ExecuteDeleteAsync();

            return rowsAffected > 0;
        }

        public async Task<PagedResult<Product>> GetPagedAsync(PaginationParams pagination)
        {
            var query = _context.Products.AsQueryable();

            var totalRecords = await query.CountAsync();

            var items = await query
                .AsNoTracking() // ⭐ PERFORMANCE
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PagedResult<Product>
            {
                Items = items,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalRecords = totalRecords
            };
        }
    }
}
