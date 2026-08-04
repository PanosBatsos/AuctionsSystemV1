using AuctionsSystem.AccountService.Application.Abstractions.Logging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Application.Features.GetProfile
{
    public record GetAccountQuery(Guid Id) : IRequest<UserProfileDto>, ILoggableRequest;
}
