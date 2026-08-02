using AuctionsSystem.AccountService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Abstractions.Security
{
    public interface ITokenProvider
    {
        string GenerateToken(Guid accountId, string username, UserRole role);
    }
}
