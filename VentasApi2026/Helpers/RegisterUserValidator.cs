using FluentValidation;
using VentasApi2026.DTOs;

namespace VentasApi2026.Helpers
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username es requerido")
                .MinimumLength(4).WithMessage("Mínimo 4 caracteres");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password es requerido")
                .MinimumLength(8).WithMessage("Mínimo 8 caracteres")
                .Matches("[A-Z]").WithMessage("Debe tener al menos una mayúscula")
                .Matches("[a-z]").WithMessage("Debe tener al menos una minúscula")
                .Matches("[0-9]").WithMessage("Debe tener al menos un número")
                .Matches("[^a-zA-Z0-9]").WithMessage("Debe tener al menos un carácter especial");

            //RuleFor(x => x.Role)
            //    .Must(r => r == "Seller" || r == "Admin")
            //    .WithMessage("Role debe ser Seller o Admin");
        }
    }
}
