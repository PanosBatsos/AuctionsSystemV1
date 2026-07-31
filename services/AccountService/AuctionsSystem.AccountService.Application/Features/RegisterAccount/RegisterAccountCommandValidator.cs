using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Features.RegisterAccount
{
    public class RegisterAccountCommandValidator : AbstractValidator<RegisterAccountCommand>
    {
        public RegisterAccountCommandValidator() 
        {
            RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(50).WithMessage("Username must not exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.")
                .MaximumLength(150).WithMessage("Email must not exceed 150 characters.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

            RuleFor(x => x.IdNumber)
                .NotEmpty().WithMessage("ID number (or Passport) is required.")
                .MaximumLength(20).WithMessage("ID number must not exceed 20 characters.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Length(10).WithMessage("Phone number must be exactly 10 digits long.")
                .Matches(@"^[0-9]+$").WithMessage("Phone number must contain only numbers.");
        }
    }
}
