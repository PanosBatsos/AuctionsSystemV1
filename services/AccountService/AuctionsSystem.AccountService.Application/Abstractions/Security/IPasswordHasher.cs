using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Abstractions.Security
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool verify(string password, string passwordHash);
    }
}
