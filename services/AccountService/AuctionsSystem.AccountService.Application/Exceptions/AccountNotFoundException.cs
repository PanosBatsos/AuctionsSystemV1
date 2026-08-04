using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Exceptions
{
    public class AccountNotFoundException : Exception
    {
        public AccountNotFoundException(string message = "Account not found") : base(message){ }
    }
}
