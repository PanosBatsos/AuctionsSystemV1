using AuctionsSystem.AccountService.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Features.LoginAccount
{
    public record LoginAccountCommand(
        string Email,
        string Password
        ) : IRequest<AuthenticationResponseDto>;
}
