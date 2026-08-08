using AuctionsSystem.AccountService.Application.Abstractions.Persistence;
using AuctionsSystem.AccountService.Application.Exceptions;
using AuctionsSystem.AccountService.Application.Features.UpdateAccount.UpdateUsername;
using AuctionsSystem.AccountService.Domain.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Tests.Features.UpdateAccount.UpdateUsername
{
    public class UpdateUsernameCommandHandlerTests
    {
        private readonly Mock<IAccountRepository> _repositoryMock = new();

        private UpdateUsernameCommandHandler CreateSut() => new(_repositoryMock.Object);

        [Fact]
        public async Task Handle_ShouldUpdateUsername_WhenRequestIsValid()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var command = new UpdateUsernameCommand(accountId, "newUsername");

            var existingAccount = Account.CreateInitialAccount(
                "oldUsername", "test@example.com", "hash", "Nikos", "Pappas", "6987654321", "AN123456");

            _repositoryMock
                .Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingAccount);

            _repositoryMock
                .Setup(repo => repo.IsUsernameTakenAsync(command.NewUsername, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var sut = CreateSut();

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            existingAccount.UserName.Should().Be(command.NewUsername);

            _repositoryMock.Verify(repo => repo.UpdateAsync(existingAccount, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowAccountNotFoundException_WhenAccountDoesNotExist()
        {
            // Arrange
            var command = new UpdateUsernameCommand(Guid.NewGuid(), "newUsername");

            _repositoryMock
                .Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Account?)null);

            var sut = CreateSut();

            // Act
            Func<Task> act = async () => await sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<AccountNotFoundException>();

            _repositoryMock.Verify(repo => repo.IsUsernameTakenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnEarly_WhenNewUsernameIsSameAsCurrent()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var command = new UpdateUsernameCommand(accountId, "sameUsername");

            var existingAccount = Account.CreateInitialAccount(
                "sameUsername", "test@example.com", "hash", "Nikos", "Pappas", "6987654321", "AN123456");

            _repositoryMock
                .Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingAccount);

            var sut = CreateSut();

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(repo => repo.IsUsernameTakenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowPropertyAlreadyInUseException_WhenUsernameIsTaken()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var command = new UpdateUsernameCommand(accountId, "takenUsername");

            var existingAccount = Account.CreateInitialAccount(
                "oldUsername", "test@example.com", "hash", "Nikos", "Pappas", "6987654321", "AN123456");

            _repositoryMock
                .Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingAccount);

            _repositoryMock
                .Setup(repo => repo.IsUsernameTakenAsync(command.NewUsername, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var sut = CreateSut();

            // Act
            Func<Task> act = async () => await sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<PropertyAlreadyInUseException>()
                .Where(e => e.FieldName == "Username"); 

            _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Account>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
