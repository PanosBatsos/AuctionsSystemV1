using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.DTOs
{
    public record RegisterAccountCommandResponseDto(Guid Id, string Email, string PhoneNumber);
    
}
