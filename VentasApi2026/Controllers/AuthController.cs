using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VentasApi2026.Common;
using VentasApi2026.DTOs;
using VentasApi2026.Services;
using VentasApi2026.Services.Interfaces;

namespace VentasApi2026.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly AuthService _authService;
        private readonly ICurrentUser _currentUser;

        public AuthController(AuthService authService, ICurrentUser currentUser)
        {
            _authService = authService;
            _currentUser = currentUser;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(ApiResponse<AuthResponseDto>.Ok(result));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(TokenRefreshRequest request)
        {
            await _authService.LogoutAsync(request.RefreshToken);
            return Ok(ApiResponse<string>.Ok("Logged out successfully"));
        }

        //[Authorize(Policy = "CanManageUsers")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
        {
            await _authService.RegisterAsync(dto, _currentUser.Id);
            return Ok();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenRefreshRequest request)
        {
            var result = await _authService.RefreshAsync(request);
            return Ok(ApiResponse<AuthResponseDto>.Ok(result));
        }
    }
}
