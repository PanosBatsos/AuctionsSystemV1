namespace AuctionsSystem.AccountService.Api.DTOs.RegisterAccount
{
    public record RegisterAccountResponseDto(
        Guid Id,
        string Email,
        string PhoneNumber,
        string Message
    );
}
