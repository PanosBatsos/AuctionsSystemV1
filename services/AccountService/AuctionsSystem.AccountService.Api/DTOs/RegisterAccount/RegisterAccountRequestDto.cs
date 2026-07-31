namespace AuctionsSystem.AccountService.Api.DTOs.RegisterAccount
{
    public record RegisterAccountRequestDto(
        string Username,
        string Email,
        string Password,
        string FirstName,
        string LastName,
        string PhoneNumber,
        string IdNumber
    );
    
}
