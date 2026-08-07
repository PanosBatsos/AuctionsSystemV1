using AuctionsSystem.AccountService.Application.Events.RevokeToken;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Polly;
using Polly.Registry;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Tests.Events.RevokeToken
{
    public class RevokeTokenEventHandlerTests
    {
        private readonly Mock<IDistributedCache> _cacheMock = new();
        private readonly Mock<ILogger<RevokeTokenEventHandler>> _loggerMock = new();
        private readonly Mock<ResiliencePipelineProvider<string>> _pipelineProviderMock = new();

        public RevokeTokenEventHandlerTests()
        {
            _pipelineProviderMock
                .Setup(p => p.GetPipeline("redis-retry"))
                .Returns(ResiliencePipeline.Empty);
        }

        private RevokeTokenEventHandler CreateSut() => new(
            _cacheMock.Object,
            _loggerMock.Object,
            _pipelineProviderMock.Object);

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

        [Fact]
        public async Task Handle_WhenCacheFails_ShouldBubbleUpException()
        {
            // Arrange
            var sut = CreateSut();
            var tokenId = Guid.NewGuid().ToString();
            var notification = new RevokeTokenEvent(tokenId);

            var expectedException = new Exception("Redis connection timed out");

            _cacheMock.Setup(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                sut.Handle(notification, CancellationToken.None));

            Assert.Equal(expectedException.Message, exception.Message);
        }
    }
}

