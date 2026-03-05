using VentasApi2026.Common;
using VentasApi2026.DTOs;

namespace VentasApi2026.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> GetByIdAsync(int id, int userId, string role);
        Task<IEnumerable<OrderDto>> GetMyOrdersAsync(int userId, string role);
        Task<IEnumerable<OrderDto>> GetAllOrders();
        Task<PagedResult<OrderDto>> GetFilteredAsync(OrderQueryParameters query, int userId, string role);
        Task<OrderDto> CreateAsync(CreateOrderDto dto, int userId);
        Task CompleteAsync(int id, int userId, string role);
        Task CancelAsync(int id, int userId, string role);
    }
}
