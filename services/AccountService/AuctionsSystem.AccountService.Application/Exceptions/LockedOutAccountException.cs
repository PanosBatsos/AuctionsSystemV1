using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Exceptions
{
    public class LockedOutAccountException : Exception
    {
        public LockedOutAccountException(string message = "The account is temporarily locked due to multiple failed login attempts.")
            : base(message)
        {
        }
    }
}
