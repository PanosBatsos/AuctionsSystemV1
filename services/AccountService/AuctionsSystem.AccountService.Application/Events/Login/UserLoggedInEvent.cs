using AuctionsSystem.AccountService.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Events.Login
{
    public record UserLoggedInEvent(Account Account) : INotification;
   
}
