using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Events.RevokeToken
{
    public record RevokeTokenEvent(string TokenId) : INotification;
}
