using System.ComponentModel.DataAnnotations;

namespace VentasApi2026.DTOs
{
    public class LoginDto
    {
        [Required (ErrorMessage ="El campo {0} es requerido")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Password { get; set; } = null!;
    }
}
