using AuctionsSystem.AccountService.Api.ExceptionHandling;
using AuctionsSystem.AccountService.Application.Abstractions.Persistence;
using AuctionsSystem.AccountService.Application.Abstractions.Security;
using AuctionsSystem.AccountService.Application.Exceptions;
using AuctionsSystem.AccountService.Application.Features.LoginAccount;
using AuctionsSystem.AccountService.Domain.Entities;
using AuctionsSystem.AccountService.Domain.Enums;
using FluentAssertions;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Tests.Features.LoginAccount
{
    public class LoginAccountCommandHandlerTests
    {
        private readonly Mock<IAccountRepository> _repositoryMock = new();
        private readonly Mock<IPasswordHasher> _hasherMock = new();
        private readonly Mock<ITokenProvider> _tokenProviderMock = new();
        private readonly Mock<IPublisher> _publisherMock = new();

        private LoginAccountCommandHandler CreateSut() => new(
            _tokenProviderMock.Object,
            _repositoryMock.Object,
            _hasherMock.Object,
            _publisherMock.Object
        );

        private Account CreateValidAccount()
        {
            return Account.CreateInitialAccount(
                "giorgos.georgiou",
                "giorgos@example.com",
                "hashed_password",
                "Georgios",
                "Georgiou",
                "6912345678",
                "AB123456");
        }

        [Fact]
        public async Task Handle_ShouldReturnToken_WhenCredentialsAreValidAndAccountIsActive()
        {
            // Arrange
            var command = new LoginAccountCommand("giorgos@example.com", "CorrectPassword123!", "192.168.1.1");
            var account = CreateValidAccount();
            var expectedToken = "mocked_jwt_token_string";

            _repositoryMock
                .Setup(repo => repo.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            _hasherMock
                .Setup(hasher => hasher.Verify(command.Password, account.Security.PasswordHash))
                .Returns(true);

            _tokenProviderMock
                .Setup(tp => tp.GenerateToken(account.Id, account.UserName, account.Role))
                .Returns(expectedToken);

            var sut = CreateSut();

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().Be(expectedToken);
            result.Email.Should().Be(account.Email);
            result.Username.Should().Be(account.UserName);

            _repositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            account.LastLoginIp.Should().Be(command.IpAddress); 
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidCredentialsException_WhenAccountDoesNotExist()
        {
            // Arrange
            var command = new LoginAccountCommand("notfound@example.com", "Password123!", "192.168.1.1");

            _repositoryMock
                .Setup(repo => repo.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account?)null);

            var sut = CreateSut();

            // Act
            Func<Task> act = async () => await sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidCredentialsException>();

            _repositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _tokenProviderMock.Verify(tp => tp.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<UserRole>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowInvalidCredentialsExceptionAndSaveFailure_WhenPasswordIsIncorrect()
        {
            // Arrange
            var command = new LoginAccountCommand("giorgos@example.com", "WrongPassword!", "192.168.1.1");
            var account = CreateValidAccount();

            _repositoryMock
                .Setup(repo => repo.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            _hasherMock
                .Setup(hasher => hasher.Verify(command.Password, account.Security.PasswordHash))
                .Returns(false);

            var sut = CreateSut();

            // Act
            Func<Task> act = async () => await sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidCredentialsException>();

 
            _repositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            account.Security.AccessFailedCount.Should().Be(1); 
            _tokenProviderMock.Verify(tp => tp.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<UserRole>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowLockedOutAccountException_WhenAccountIsLockedOut()
        {
            // Arrange
            var command = new LoginAccountCommand("giorgos@example.com", "CorrectPassword123!", "192.168.1.1");
            var account = CreateValidAccount();


            account.RecordLoginFailure(1, TimeSpan.FromMinutes(15));

            _repositoryMock
                .Setup(repo => repo.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            _hasherMock
                .Setup(hasher => hasher.Verify(command.Password, account.Security.PasswordHash))
                .Returns(true);

            var sut = CreateSut();

            // Act
            Func<Task> act = async () => await sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<LockedOutAccountException>();

            _tokenProviderMock.Verify(tp => tp.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<UserRole>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowInactiveAccountException_WhenAccountIsInactive()
        {
            // Arrange
            var command = new LoginAccountCommand("giorgos@example.com", "CorrectPassword123!", "192.168.1.1");
            var account = CreateValidAccount();

            account.DeactivateAccount();

            _repositoryMock
                .Setup(repo => repo.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);

            _hasherMock
                .Setup(hasher => hasher.Verify(command.Password, account.Security.PasswordHash))
                .Returns(true);

            var sut = CreateSut();

            // Act
            Func<Task> act = async () => await sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InactiveAccountException>();

            _tokenProviderMock.Verify(tp => tp.GenerateToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<UserRole>()), Times.Never);
        }
    }
}
