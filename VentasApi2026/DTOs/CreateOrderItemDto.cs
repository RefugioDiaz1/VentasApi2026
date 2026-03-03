using System.ComponentModel.DataAnnotations;

namespace VentasApi2026.DTOs
{
    public class CreateOrderItemDto
    {
        [Required (ErrorMessage ="El campo {0} es Requerido")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "El campo {0} es Requerido")]
        [Range(1, 99999, ErrorMessage = "EL precio debe ser mayor a {1}")]
        public int Quantity { get; set; }
    }
}
