using Microsoft.EntityFrameworkCore;
using VentasApi2026.Data;
using VentasApi2026.Exceptions;
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
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders.Include(w=>w.Details).FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<IEnumerable<Order>?> GetMyOrdersAsync(int idUser)
        {
            return await _context.Orders.Include(w => w.Details).Where(w=>w.CreatedByUserId == idUser).ToListAsync();
        }

        public async Task<IEnumerable<Order>?> GetAllOrders()
        {
            return await _context.Orders.Include(w => w.Details).ToListAsync();
        }

        public IQueryable<Order> Query()
        {
            return _context.Orders
                .Include(o => o.Details);
        }
    }
}
