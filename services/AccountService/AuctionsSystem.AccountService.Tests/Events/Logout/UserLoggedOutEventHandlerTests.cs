using AuctionsSystem.AccountService.Application.Events.Logout;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Polly;
using Polly.Registry;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Tests.Events.Logout
{
    public class UserLoggedOutEventHandlerTests
    {
        private readonly Mock<IDistributedCache> _cacheMock = new();
        private readonly Mock<ILogger<UserLoggedOutEventHandler>> _loggerMock = new();
        private readonly Mock<ResiliencePipelineProvider<string>> _pipelineProviderMock = new();

        public UserLoggedOutEventHandlerTests()
        {
            _pipelineProviderMock
                .Setup(p => p.GetPipeline("redis-retry"))
                .Returns(ResiliencePipeline.Empty);
        }

        private UserLoggedOutEventHandler CreateSut() => new(
            _cacheMock.Object,
            _loggerMock.Object,
            _pipelineProviderMock.Object);

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

        [Fact]
        public async Task Handle_WhenCacheFails_ShouldBubbleUpException()
        {
            // Arrange
            var sut = CreateSut();
            var accountId = Guid.NewGuid();
            var notification = new UserLoggedOutEvent(accountId);

            var expectedException = new Exception("Redis is unreachable");

            _cacheMock.Setup(c => c.RemoveAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                sut.Handle(notification, CancellationToken.None));

            Assert.Equal(expectedException.Message, exception.Message);
        }
    }
}
