using VentasApi2026.DTOs;
using VentasApi2026.Models;

namespace VentasApi2026.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task RegisterAsync(RegisterUserDto dto, int idUser);
        Task<AuthResponseDto> RefreshAsync(TokenRefreshRequest request);
        Task LogoutAsync(string refreshToken);
    }
}
