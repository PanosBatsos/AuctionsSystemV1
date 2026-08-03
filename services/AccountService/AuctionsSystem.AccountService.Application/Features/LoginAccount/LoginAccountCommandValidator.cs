using FluentValidation;


namespace AuctionsSystem.AccountService.Application.Features.LoginAccount
{
    public class LoginAccountCommandValidator : AbstractValidator<LoginAccountCommand>
    {
        public LoginAccountCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");

            RuleFor(x => x.IpAddress)
                .NotEmpty().WithMessage("IP Address is required.")
                .NotEqual("unknown").WithMessage("Could not determine the client IP address.");
        }
    }
}
