namespace VentasApi2026.DTOs
{
    public class UpdateProductDto
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
