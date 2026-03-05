using System.ComponentModel.DataAnnotations;

namespace VentasApi2026.DTOs
{
    public class RegisterUserDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
