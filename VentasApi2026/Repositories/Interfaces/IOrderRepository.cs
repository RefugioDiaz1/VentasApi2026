using VentasApi2026.DTOs;
using VentasApi2026.Models;

namespace VentasApi2026.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id);
        Task<IEnumerable<Order>?> GetMyOrdersAsync(int idUser);
        Task<IEnumerable<Order>?> GetAllOrders();
        IQueryable<Order> Query();
        Task AddAsync(Order order);
    }
}
