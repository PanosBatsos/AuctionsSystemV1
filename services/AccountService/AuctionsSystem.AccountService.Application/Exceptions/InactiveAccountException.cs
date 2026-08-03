using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Exceptions
{
    public class InactiveAccountException : Exception
    {
        public InactiveAccountException(string message = "The account is inactive. Please contact support.")
            : base(message)
        {
        }
    }
}
