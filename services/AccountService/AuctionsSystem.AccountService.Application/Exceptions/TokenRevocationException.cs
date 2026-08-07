using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Exceptions
{
    public class TokenRevocationException : Exception
    {
        public TokenRevocationException(string message = "An error occurred while revoking the token.")
            : base(message)
        {
        }

        public TokenRevocationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
