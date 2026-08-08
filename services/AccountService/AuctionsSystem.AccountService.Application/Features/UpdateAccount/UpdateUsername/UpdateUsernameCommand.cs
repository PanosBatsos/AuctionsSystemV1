using AuctionsSystem.AccountService.Application.Abstractions.Logging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Features.UpdateAccount.UpdateUsername
{
    public record UpdateUsernameCommand(Guid Id, string NewUsername) : IRequest, ILoggableRequest;
    
}
