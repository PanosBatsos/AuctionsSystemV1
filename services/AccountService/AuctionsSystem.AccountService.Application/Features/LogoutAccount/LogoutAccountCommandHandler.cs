using AuctionsSystem.AccountService.Application.Events.Logout;
using AuctionsSystem.AccountService.Application.Events.RevokeToken;
using AuctionsSystem.AccountService.Application.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;


namespace AuctionsSystem.AccountService.Application.Features.LogoutAccount
{
    public class LogoutAccountCommandHandler : IRequestHandler<LogoutAccountCommand>
    {
        private readonly IPublisher _publisher;

        public LogoutAccountCommandHandler(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public async Task Handle(LogoutAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _publisher.Publish(new RevokeTokenEvent(request.TokenId), cancellationToken);
            }
            catch (Exception ex)
            {
                throw new TokenRevocationException("There was an error in token revokation", ex);
            }

            try
            {
                await _publisher.Publish(new UserLoggedOutEvent(request.Id), cancellationToken);
            }
            catch (Exception) { }
        }
    }
}
