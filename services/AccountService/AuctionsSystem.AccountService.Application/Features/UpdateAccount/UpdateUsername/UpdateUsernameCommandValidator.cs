using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Features.UpdateAccount.UpdateUsername
{
    public class UpdateUsernameCommandValidator : AbstractValidator<UpdateUsernameCommand>
    {
        public UpdateUsernameCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id must not be empty");


            RuleFor(x => x.NewUsername)
                .NotEmpty().WithMessage("New username must not be empty")
                .MaximumLength(50).WithMessage("Username must not overtake 50 characters");
        }
    }
}
