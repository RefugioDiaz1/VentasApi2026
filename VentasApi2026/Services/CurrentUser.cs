using System.Security.Claims;
using VentasApi2026.Services.Interfaces;

namespace VentasApi2026.Services
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContext;

        public CurrentUser(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public int Id => int.Parse(
            _httpContext.HttpContext!.User
                .FindFirst(ClaimTypes.NameIdentifier)!.Value);

        public string Role => _httpContext.HttpContext!.User
            .FindFirst(ClaimTypes.Role)!.Value;
    }
}
