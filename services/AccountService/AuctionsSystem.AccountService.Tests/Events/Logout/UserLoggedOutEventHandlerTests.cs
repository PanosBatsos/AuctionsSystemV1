using AuctionsSystem.AccountService.Application.Events.Logout;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Tests.Events.Logout
{
    public class UserLoggedOutEventHandlerTests
    {
        private readonly Mock<IDistributedCache> _cacheMock = new();
        private readonly Mock<ILogger<UserLoggedOutEventHandler>> _loggerMock = new();

        private UserLoggedOutEventHandler CreateSut() => new(
            _cacheMock.Object,
            _loggerMock.Object);

        [Fact]
        public async Task Handle_ShouldRemoveAccountFromCache_WhenEventIsReceived()
        {
            // Arrange
            var sut = CreateSut();

            var accountId = Guid.NewGuid();

            var notification = new UserLoggedOutEvent(accountId);

            var expectedCacheKey = $"account-{notification.Id}";

            // Act
            await sut.Handle(notification, CancellationToken.None);

            // Assert
            _cacheMock.Verify(c => c.RemoveAsync(
                expectedCacheKey,
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
