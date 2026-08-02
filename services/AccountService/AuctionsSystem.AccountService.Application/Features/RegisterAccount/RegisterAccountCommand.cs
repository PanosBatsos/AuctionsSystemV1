using AuctionsSystem.AccountService.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Features.RegisterAccount
{
    public record RegisterAccountCommand(
        string Username,
        string Email,
        string FirstName,
        string LastName,
        string Password,
        string IdNumber,
        string PhoneNumber
        ) : IRequest<AuthenticationResponseDto>;
}
