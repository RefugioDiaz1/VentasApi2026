using VentasApi2026.DTOs;

namespace VentasApi2026.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> GetByIdAsync(int id);
        Task<OrderDto> CreateAsync(CreateOrderDto dto);
        Task CompleteAsync(int id);
        Task CancelAsync(int id);
    }
}
