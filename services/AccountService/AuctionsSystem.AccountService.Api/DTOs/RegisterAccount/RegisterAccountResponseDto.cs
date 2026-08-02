namespace AuctionsSystem.AccountService.Api.DTOs.RegisterAccount
{
    public record RegisterAccountResponseDto(
        string Username,
        string Email,
        string Token,
        string Message
    );
}
