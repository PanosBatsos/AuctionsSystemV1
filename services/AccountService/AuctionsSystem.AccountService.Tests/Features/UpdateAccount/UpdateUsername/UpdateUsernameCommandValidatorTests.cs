using AuctionsSystem.AccountService.Application.Features.UpdateAccount.UpdateUsername;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Tests.Features.UpdateAccount.UpdateUsername
{
    public class UpdateUsernameCommandValidatorTests
    {
        private readonly UpdateUsernameCommandValidator _validator = new();
        [Fact]
        public void Validate_ShouldHaveError_WhenIdIsEmpty()
        {
            // Arrange
            var command = new UpdateUsernameCommand(Guid.Empty, "newUsername");

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNewUsernameIsEmpty()
        {
            // Arrange
            var command = new UpdateUsernameCommand(Guid.NewGuid(), "");

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.NewUsername);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenNewUsernameExceedsMaxLength()
        {
            // Arrange
            var longUsername = new string('a', 51); 
            var command = new UpdateUsernameCommand(Guid.NewGuid(), longUsername);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.NewUsername);
        }

        [Fact]
        public void Should_NotHaveError_WhenRequestIsValid()
        {
            // Arrange
            var command = new UpdateUsernameCommand(Guid.NewGuid(), "validUsername123");

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
