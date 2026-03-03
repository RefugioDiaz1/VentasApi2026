using VentasApi2026.Models;

namespace VentasApi2026.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
        public OrderStatus Status { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }
}
