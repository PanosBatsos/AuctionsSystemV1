using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Exceptions
{
    public class NotVerifiedAccountException : Exception
    {
        public NotVerifiedAccountException(string message = "The account is not verified. Please verify your email and phone number.")
            : base(message)
        {
        }
    }
}
