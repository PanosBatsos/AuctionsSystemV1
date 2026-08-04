using AuctionsSystem.AccountService.Domain.Enums;

namespace AuctionsSystem.AccountService.Api.DTOs.GetAccount
{
    public record GetAccountResponseDto(
        Guid Id,
        string Username,
        string Email,
        string FirstName,
        string LastName,
        string PhoneNumber,
        string IdNumber,
        UserRole Role);


}
