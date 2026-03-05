using System.ComponentModel.DataAnnotations;

namespace VentasApi2026.DTOs
{
    public class UpdateProductDto
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, 99999, ErrorMessage = "EL precio debe ser mayor a {1}")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }
    }
}
