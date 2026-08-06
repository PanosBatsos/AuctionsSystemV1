using AuctionsSystem.AccountService.Application.Events.RevokeToken;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Tests.Events.RevokeToken
{
    public class RevokeTokenEventHandlerTests
    {
        private readonly Mock<IDistributedCache> _cacheMock = new();
        private readonly Mock<ILogger<RevokeTokenEventHandler>> _loggerMock = new();

        private RevokeTokenEventHandler CreateSut() => new(
            _cacheMock.Object,
            _loggerMock.Object);

        [Fact]
        public async Task Handle_ShouldAddTokenToBlacklist_WithCorrectExpiration()
        {
            // Arrange
            var sut = CreateSut();

            var tokenId = Guid.NewGuid().ToString();
            var notification = new RevokeTokenEvent(tokenId);

            var expectedCacheKey = $"blacklist-{notification.TokenId}";
            var expectedExpiration = TimeSpan.FromMinutes(15);

            // Act
            await sut.Handle(notification, CancellationToken.None);

            // Assert

            _cacheMock.Verify(c => c.SetAsync(
                expectedCacheKey,
                It.IsAny<byte[]>(),
                It.Is<DistributedCacheEntryOptions>(opt =>
                    opt.AbsoluteExpirationRelativeToNow == expectedExpiration),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
