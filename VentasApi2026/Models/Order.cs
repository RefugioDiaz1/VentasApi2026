namespace VentasApi2026.Models
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public decimal Total { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        // Navigation property
        public ICollection<OrderDetail> Details { get; set; }
            = new List<OrderDetail>();
    }
}
