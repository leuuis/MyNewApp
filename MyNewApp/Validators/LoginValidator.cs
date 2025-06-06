using MyNewApp.DTOs;
using FluentValidation;
using System.Data;

namespace MyNewApp.Validators
{
    public class LoginValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginValidator()
        {
            RuleSet("Login", () =>
            {
                RuleFor(x => x.Username)
                    .NotEmpty().WithMessage("Username is required on token")
                    .MinimumLength(3)
                    .MaximumLength(15);

                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Password is required on token")
                    .MinimumLength(6).WithMessage("The password shuld be at least 6 caracteres")
                    .MaximumLength(12).WithMessage("The password shuld be a max of 12 caracteres");
            });
        }
    }
}