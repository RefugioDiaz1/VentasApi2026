using System.ComponentModel.DataAnnotations;

namespace VentasApi2026.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
