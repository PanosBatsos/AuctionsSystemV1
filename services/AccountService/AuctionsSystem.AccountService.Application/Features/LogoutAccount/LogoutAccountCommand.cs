

using AuctionsSystem.AccountService.Application.Abstractions.Logging;
using MediatR;

namespace AuctionsSystem.AccountService.Application.Features.LogoutAccount
{
    public record LogoutAccountCommand(Guid Id, string TokenId) : IRequest, ILoggableRequest;
    
}
