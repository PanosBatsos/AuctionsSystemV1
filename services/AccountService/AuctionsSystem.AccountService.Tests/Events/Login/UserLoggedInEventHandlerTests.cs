using AuctionsSystem.AccountService.Application.DTOs;
using AuctionsSystem.AccountService.Application.Events.Login;
using AuctionsSystem.AccountService.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AuctionsSystem.AccountService.Tests.Events.Login
{
    public class UserLoggedInEventHandlerTests
    {
        private readonly Mock<IDistributedCache> _cacheMock = new();
        private readonly Mock<ILogger<UserLoggedInEventHandler>> _loggerMock = new();

        private UserLoggedInEventHandler CreateSut() => new(
            _cacheMock.Object,
            _loggerMock.Object
        );

        [Fact]
        public async Task Handle_ShouldSaveAccountDtoToCache_WithCorrectKeyAndExpiration()
        {
            // Arrange
            var account = Account.CreateInitialAccount(
                "giorgos.georgiou",
                "giorgos@example.com",
                "hashed_password",
                "Georgios",
                "Georgiou",
                "6912345678",
                "AB123456");

            var notification = new UserLoggedInEvent(account);
            var expectedCacheKey = $"account-{account.Id}";

            
            var sut = CreateSut();

            // Act
            await sut.Handle(notification, CancellationToken.None);

            // Assert
            _cacheMock.Verify(
                x => x.SetAsync(
                    expectedCacheKey,
                    It.IsAny<byte[]>(),
                    It.Is<DistributedCacheEntryOptions>(opts =>
                        opts.AbsoluteExpirationRelativeToNow == TimeSpan.FromMinutes(15)),
                    It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_ShouldSerializeTheCorrectDtoToCache()
        {
            // Arrange
            var account = Account.CreateInitialAccount(
                "giorgos.georgiou",
                "giorgos@example.com",
                "hashed_password",
                "Georgios",
                "Georgiou",
                "6912345678",
                "AB123456");

            var notification = new UserLoggedInEvent(account);
            var expectedCacheKey = $"account-{account.Id}";

            byte[] savedBytes = null!;

            _cacheMock.Setup(x => x.SetAsync(
                expectedCacheKey,
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>(
                (key, bytes, options, token) => savedBytes = bytes)
            .Returns(Task.CompletedTask);

            var sut = CreateSut();

            // Act
            await sut.Handle(notification, CancellationToken.None);

            // Assert
            savedBytes.Should().NotBeNull();

            var savedJson = Encoding.UTF8.GetString(savedBytes);
            var savedDto = JsonSerializer.Deserialize<GetAccountQueryResponseDto>(savedJson);

            savedDto.Should().NotBeNull();
            savedDto!.Id.Should().Be(account.Id);
            savedDto.Email.Should().Be(account.Email);
            savedDto.Username.Should().Be(account.UserName);
            savedDto.FirstName.Should().Be(account.FirstName);
        }
    }
}
