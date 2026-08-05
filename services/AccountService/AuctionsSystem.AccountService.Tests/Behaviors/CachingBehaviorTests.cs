using AuctionsSystem.AccountService.Application.Abstractions.Cache;
using AuctionsSystem.AccountService.Application.Behaviors;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AuctionsSystem.AccountService.Tests.Behaviors
{
    // Dummies
    public record TestResponse(string Message);

    public record TestStandardRequest : IRequest<TestResponse>;

    public record TestCacheableRequest : IRequest<TestResponse>, ICacheable
    {
        public string CacheKey => "test-cache-key";
        public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
    }

    public class CachingBehaviorTests
    {
        private readonly Mock<IDistributedCache> _mockCache;
        private readonly Mock<ILogger<CachingBehavior<IRequest<TestResponse>, TestResponse>>> _mockLogger;

        public CachingBehaviorTests()
        {
            _mockCache = new Mock<IDistributedCache>();
            _mockLogger = new Mock<ILogger<CachingBehavior<IRequest<TestResponse>, TestResponse>>>();
        }

        [Fact]
        public async Task Handle_ShouldCallNextDirectly_WhenRequestIsNotCacheable()
        {
            // Arrange
            var request = new TestStandardRequest();
            var expectedResponse = new TestResponse("Data from DB");

            var nextDelegateMock = new Mock<RequestHandlerDelegate<TestResponse>>();
            nextDelegateMock.Setup(x => x()).ReturnsAsync(expectedResponse);

            // System Under Test
            var sut = new CachingBehavior<TestStandardRequest, TestResponse>(_mockCache.Object, null!);

            // Act
            var result = await sut.Handle(request, nextDelegateMock.Object, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResponse);
            nextDelegateMock.Verify(x => x(), Times.Once); 
            _mockCache.Verify(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never); 
        }

        [Fact]
        public async Task Handle_ShouldReturnCachedData_WhenCacheHit()
        {
            // Arrange
            var request = new TestCacheableRequest();
            var cachedResponse = new TestResponse("Data from Cache");
            var cachedResponseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(cachedResponse));

            _mockCache.Setup(x => x.GetAsync(request.CacheKey, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(cachedResponseBytes);

            var nextDelegateMock = new Mock<RequestHandlerDelegate<TestResponse>>();


            // System Under Test
            var mockLogger = new Mock<ILogger<CachingBehavior<TestCacheableRequest, TestResponse>>>();
            var sut = new CachingBehavior<TestCacheableRequest, TestResponse>(_mockCache.Object, mockLogger.Object);

            // Act
            var result = await sut.Handle(request, nextDelegateMock.Object, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(cachedResponse);
            nextDelegateMock.Verify(x => x(), Times.Never); 
        }

        [Fact]
        public async Task Handle_ShouldCallNextAndSaveToCache_WhenCacheMiss()
        {
            // Arrange
            var request = new TestCacheableRequest();
            var dbResponse = new TestResponse("Data from DB");

            _mockCache.Setup(x => x.GetAsync(request.CacheKey, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((byte[])null!);

            var nextDelegateMock = new Mock<RequestHandlerDelegate<TestResponse>>();
            nextDelegateMock.Setup(x => x()).ReturnsAsync(dbResponse);

            // System Under Test
            var mockLogger = new Mock<ILogger<CachingBehavior<TestCacheableRequest, TestResponse>>>();
            var sut = new CachingBehavior<TestCacheableRequest, TestResponse>(_mockCache.Object, mockLogger.Object);

            // Act
            var result = await sut.Handle(request, nextDelegateMock.Object, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(dbResponse);
            nextDelegateMock.Verify(x => x(), Times.Once); 

            _mockCache.Verify(x => x.SetAsync(
                request.CacheKey,
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}