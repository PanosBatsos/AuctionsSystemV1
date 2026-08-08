using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Abstractions.Cache
{
    public interface ICacheInvalidator
    {
        string CacheKey { get; }
    }
}
