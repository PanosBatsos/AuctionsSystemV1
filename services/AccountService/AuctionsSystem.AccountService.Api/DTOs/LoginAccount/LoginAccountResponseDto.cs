namespace AuctionsSystem.AccountService.Api.DTOs.LoginAccount
{
    public record LoginAccountResponseDto(
        string Username,
        string Email,
        string Token,
        string Message);
    
}
