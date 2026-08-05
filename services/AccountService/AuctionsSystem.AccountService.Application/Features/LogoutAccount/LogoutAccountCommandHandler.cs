using AuctionsSystem.AccountService.Application.Events.Logout;
using MediatR;


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
            await _publisher.Publish(new UserLoggedOutEvent(request.Id), cancellationToken);
        }
    }
}
