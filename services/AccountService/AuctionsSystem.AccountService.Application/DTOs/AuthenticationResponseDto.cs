using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.DTOs
{
    public record AuthenticationResponseDto(
        string Username,
        string Email,
        string Token
        );
}
