namespace AuctionsSystem.AccountService.Api.ExceptionHandling
{
    public class InvalidCredentialsException : Exception  
    {
        public InvalidCredentialsException(string message = "Invalid email or password.")
            : base(message)
        {
        }
    }
}
