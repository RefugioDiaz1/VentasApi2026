using Microsoft.EntityFrameworkCore;
using VentasApi2026.Data;
using VentasApi2026.Exceptions;
using VentasApi2026.Repositories.Interfaces;

namespace VentasApi2026.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IOrderRepository Orders { get; }
        public IProductRepository Products { get; }
        public IUserRepository Users { get; }
        public IRefreshTokenRepository RefreshTokens { get; } // ✅ nuevo


        public UnitOfWork(AppDbContext context, IProductRepository products, IOrderRepository orders, IUserRepository users, IRefreshTokenRepository refreshTokens)
        {
            _context = context;
            Products = products;
            Orders = orders;
            Users = users;
            RefreshTokens = refreshTokens;
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConflictException("The record was modified by another user.");
            }
        }
    }
}
