using AuctionsSystem.AccountService.Application.Abstractions.Persistence;
using AuctionsSystem.AccountService.Application.DTOs;
using AuctionsSystem.AccountService.Application.Exceptions;
using AuctionsSystem.AccountService.Application.Features.GetAccount;
using AuctionsSystem.AccountService.Domain.Entities;
using AuctionsSystem.AccountService.Domain.Enums;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Tests.Features.GetAccount
{
    public class GetAccountQueryHandlerTests
    {
        private readonly Mock<IAccountRepository> _repositoryMock = new();

        private GetAccountQueryHandler CreateSut() => new(
            _repositoryMock.Object
        );

        [Fact]
        public async Task Handle_ShouldReturnResponseDto_WhenAccountExists()
        {
            // Arrange
            var expectedAccount = Account.CreateInitialAccount(
                "npappas",
                "npappas@example.com",
                "hashed_password_dummy",
                 "Nikos",
                "Pappas",
                "6987654321",
                "AN12345678"
            );

            var query = new GetAccountQuery(expectedAccount.Id);

            _repositoryMock
                .Setup(repo => repo.GetByIdAsync(query.Id))
                .ReturnsAsync(expectedAccount);

            var sut = CreateSut();

            // Act
            var result = await sut.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<GetAccountQueryResponseDto>();

            result.Id.Should().Be(expectedAccount.Id);
            result.Username.Should().Be(expectedAccount.UserName);
            result.Email.Should().Be(expectedAccount.Email);
            result.FirstName.Should().Be(expectedAccount.FirstName);
            result.LastName.Should().Be(expectedAccount.LastName);
            result.PhoneNumber.Should().Be(expectedAccount.PhoneNumber);
            result.IdNumber.Should().Be(expectedAccount.IdNumber);
            result.Role.Should().Be(expectedAccount.Role);

            _repositoryMock.Verify(repo => repo.GetByIdAsync(query.Id), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowAccountNotFoundException_WhenAccountDoesNotExist()
        {
            // Arrange
            var query = new GetAccountQuery(Guid.NewGuid());

            _repositoryMock
                .Setup(repo => repo.GetByIdAsync(query.Id))
                .ReturnsAsync((Account?)null);

            var sut = CreateSut();

            // Act
            Func<Task> act = async () => await sut.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<AccountNotFoundException>();

            _repositoryMock.Verify(repo => repo.GetByIdAsync(query.Id), Times.Once);
        }
    }
}
