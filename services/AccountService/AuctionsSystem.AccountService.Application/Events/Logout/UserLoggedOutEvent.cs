using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Events.Logout
{
    public record UserLoggedOutEvent(Guid Id) : INotification;
}
