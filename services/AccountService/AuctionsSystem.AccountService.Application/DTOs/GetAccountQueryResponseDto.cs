using AuctionsSystem.AccountService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.DTOs
{
    public record GetAccountQueryResponseDto(
        Guid Id,
        string Username,
        string Email,
        string FirstName,
        string LastName,
        string PhoneNumber,
        string IdNumber,
        UserRole Role);
}
