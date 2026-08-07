using AuctionsSystem.AccountService.Application.Events.Logout;
using AuctionsSystem.AccountService.Application.Events.RevokeToken;
using AuctionsSystem.AccountService.Application.Exceptions;
using AuctionsSystem.AccountService.Application.Features.LogoutAccount;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Tests.Features.LogoutAccount
{
    public class LogoutAccountCommandHandlerTests
    {
        private readonly Mock<IPublisher> _publisherMock = new();

        private LogoutAccountCommandHandler CreateSut() => new(
            _publisherMock.Object);

        [Fact]
        public async Task Handle_ShouldPublishBothEvents_WhenLogoutCommandIsHandled()
        {
            // Arrange
            var sut = CreateSut();

            var accountId = Guid.NewGuid();
            var tokenId = Guid.NewGuid().ToString(); 
            var command = new LogoutAccountCommand(accountId, tokenId);

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            _publisherMock.Verify(p => p.Publish(
                It.Is<UserLoggedOutEvent>(e => e.Id == command.Id),
                It.IsAny<CancellationToken>()),
                Times.Once);

            _publisherMock.Verify(p => p.Publish(
                It.Is<RevokeTokenEvent>(e => e.TokenId == command.TokenId),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }


        [Fact]
        public async Task Handle_WhenTokenRevocationFails_ShouldThrowTokenRevocationException()
        {
            // Arrange
            var sut = CreateSut();
            var command = new LogoutAccountCommand(Guid.NewGuid(), Guid.NewGuid().ToString());

            _publisherMock
                .Setup(p => p.Publish(It.IsAny<RevokeTokenEvent>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Redis is down"));

            // Act
            await Assert.ThrowsAsync<TokenRevocationException>(() =>
                sut.Handle(command, CancellationToken.None));

            // Assert
            _publisherMock.Verify(p => p.Publish(
                It.IsAny<UserLoggedOutEvent>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_WhenCacheInvalidationFails_ShouldNotThrowException()
        {
            // Arrange
            var sut = CreateSut();
            var command = new LogoutAccountCommand(Guid.NewGuid(), Guid.NewGuid().ToString());


            _publisherMock
                .Setup(p => p.Publish(It.IsAny<UserLoggedOutEvent>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Cache is down"));

            // Act
            var exception = await Record.ExceptionAsync(() =>
                sut.Handle(command, CancellationToken.None));

            // Assert
            Assert.Null(exception);

            _publisherMock.Verify(p => p.Publish(
                It.IsAny<RevokeTokenEvent>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}

