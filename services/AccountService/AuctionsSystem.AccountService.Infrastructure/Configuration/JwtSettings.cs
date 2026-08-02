using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Infrastructure.Configuration
{
    public class JwtSettings
    {
        public string PrivateKey { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
