using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using VentasApi2026.Repositories.Interfaces;

namespace VentasApi2026.Filter
{
    public class TokenVersionFilter : IAsyncActionFilter
    {
        private readonly IUnitOfWork _uow;

        public TokenVersionFilter(IUnitOfWork uow) => _uow = uow;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = context.HttpContext.User;

            // Si no está autenticado, pasa (login/register)
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                await next();
                return;
            }

            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var tokenVersion = int.Parse(user.FindFirst("tokenVersion")?.Value ?? "0");

            var dbUser = await _uow.Users.GetByIdAsync(userId);

            // ✅ Aquí es donde rechaza el token viejo
            if (dbUser == null || dbUser.TokenVersion != tokenVersion)
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    success = false,
                    message = "Token has been revoked"
                });
                return;
            }

            await next();
        }
    }
}
