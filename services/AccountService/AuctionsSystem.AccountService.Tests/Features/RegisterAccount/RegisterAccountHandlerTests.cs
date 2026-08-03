using AuctionsSystem.AccountService.Application.Abstractions.Persistence;
using AuctionsSystem.AccountService.Application.Abstractions.Security;
using AuctionsSystem.AccountService.Application.DTOs;
using AuctionsSystem.AccountService.Application.Exceptions;
using AuctionsSystem.AccountService.Application.Features.RegisterAccount;
using AuctionsSystem.AccountService.Domain.Entities;
using AuctionsSystem.AccountService.Domain.Enums;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Tests.Features.RegisterAccount
{
    public class RegisterAccountCommandHandlerTests
    {
        private readonly Mock<IPasswordHasher> _hasherMock = new();
        private readonly Mock<IAccountRepository> _repositoryMock = new();

        private RegisterAccountCommandHandler CreateSut() => new(
            _repositoryMock.Object,
            _hasherMock.Object
        );

        [Fact]
        public async Task Handle_ShouldReturnResponseDto_WhenRequestIsValid()
        {
            // Arrange
            var command = new RegisterAccountCommand(
                "npappas", "npappas@example.com", "Nikos", "Pappas", "Password123!", "AN123456", "6987654321");

            _repositoryMock
                .Setup(repo => repo.GetByUniqueFieldsAsync(command.Email, command.PhoneNumber, command.IdNumber))
                .ReturnsAsync((Account?)null);

            _hasherMock
                .Setup(hasher => hasher.Hash(command.Password))
                .Returns("hashed_password_123");

            var sut = CreateSut();

            // Act
            var result = await sut.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<RegisterAccountCommandResponseDto>();
            result.Id.Should().NotBeEmpty();
            result.Email.Should().Be(command.Email);
            result.PhoneNumber.Should().Be(command.PhoneNumber);

            _hasherMock.Verify(h => h.Hash(command.Password), Times.Once);
            _repositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Account>()), Times.Once);
            _repositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowPropertyAlreadyInUseException_WhenEmailExists()
        {
            // Arrange
            var command = new RegisterAccountCommand(
                "npappas", "conflict@example.com", "Nikos", "Pappas", "Password123!", "AN123456", "6987654321");

            var existingAccount = Account.CreateInitialAccount(
                "otherUser", "conflict@example.com", "hash", "John", "Doe", "6900000000", "ZZ000000");

            _repositoryMock
                .Setup(repo => repo.GetByUniqueFieldsAsync(command.Email, command.PhoneNumber, command.IdNumber))
                .ReturnsAsync(existingAccount);

            var sut = CreateSut();

            // Act
            Func<Task> act = async () => await sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<PropertyAlreadyInUseException>()
                .Where(e => e.FieldName == nameof(command.Email));

            _repositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Account>()), Times.Never);
            _repositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowPropertyAlreadyInUseException_WhenPhoneNumberExists()
        {
            // Arrange
            var command = new RegisterAccountCommand(
                "npappas", "npappas@example.com", "Nikos", "Pappas", "Password123!", "AN123456", "6987654321");

            var existingAccount = Account.CreateInitialAccount(
                "otherUser", "other@example.com", "hash", "John", "Doe", "6987654321", "ZZ000000");

            _repositoryMock
                .Setup(repo => repo.GetByUniqueFieldsAsync(command.Email, command.PhoneNumber, command.IdNumber))
                .ReturnsAsync(existingAccount);

            var sut = CreateSut();

            // Act
            Func<Task> act = async () => await sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<PropertyAlreadyInUseException>()
                .Where(e => e.FieldName == nameof(command.PhoneNumber));

            _repositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Account>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowPropertyAlreadyInUseException_WhenIdNumberExists()
        {
            // Arrange
            var command = new RegisterAccountCommand(
                "npappas", "npappas@example.com", "Nikos", "Pappas", "Password123!", "AN123456", "6987654321");

            var existingAccount = Account.CreateInitialAccount(
                "otherUser", "other@example.com", "hash", "John", "Doe", "6900000000", "AN123456");

            _repositoryMock
                .Setup(repo => repo.GetByUniqueFieldsAsync(command.Email, command.PhoneNumber, command.IdNumber))
                .ReturnsAsync(existingAccount);

            var sut = CreateSut();

            // Act
            Func<Task> act = async () => await sut.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<PropertyAlreadyInUseException>()
                .Where(e => e.FieldName == nameof(command.IdNumber));

            _repositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Account>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldHashPassword_BeforeSavingAccount()
        {
            // Arrange
            var plainPassword = "MySecretPassword123!";
            var expectedHashedPassword = "$2a$11$DummyHashedPasswordString";

            var command = new RegisterAccountCommand(
                "npappas", "npappas@example.com", "Nikos", "Pappas", plainPassword, "AN123456", "6987654321");

            _repositoryMock
                .Setup(repo => repo.GetByUniqueFieldsAsync(command.Email, command.PhoneNumber, command.IdNumber))
                .ReturnsAsync((Account?)null);

            _hasherMock
                .Setup(hasher => hasher.Hash(plainPassword))
                .Returns(expectedHashedPassword);

            var sut = CreateSut();

            // Act
            await sut.Handle(command, CancellationToken.None);

            // Assert 
            _hasherMock.Verify(hasher => hasher.Hash(plainPassword), Times.Once);

            _repositoryMock.Verify(repo => repo.AddAsync(It.Is<Account>(account =>
                account.Security.PasswordHash == expectedHashedPassword)), Times.Once);
        }
    }
}
