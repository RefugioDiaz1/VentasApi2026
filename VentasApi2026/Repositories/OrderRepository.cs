using Microsoft.EntityFrameworkCore;
using VentasApi2026.Data;
using VentasApi2026.Models;
using VentasApi2026.Repositories.Interfaces;

namespace VentasApi2026.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context) {
            this._context = context;
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders.Include(w=>w.Details).FirstOrDefaultAsync(w => w.Id == id);
        }
    }
}
