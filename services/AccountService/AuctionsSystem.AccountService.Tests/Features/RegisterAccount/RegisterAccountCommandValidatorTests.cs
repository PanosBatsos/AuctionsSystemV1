using AuctionsSystem.AccountService.Application.Features.RegisterAccount;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Tests.Features.RegisterAccount
{
    public class RegisterAccountCommandValidatorTests
    {
        private readonly RegisterAccountCommandValidator _validator = new();
    

    [Fact]
        public void Validate_ShouldHaveError_WhenUsernameIsEmpty()
        {
            var command = new RegisterAccountCommand(
                "", "valid@email.com", "John", "Doe", "Password123", "1234567890", "1234567890");

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Username);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenEmailIsInvalid()
        {
            var command = new RegisterAccountCommand(
                "user1", "invalid-email", "John", "Doe", "Password123", "1234567890", "1234567890");

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Theory]
        [InlineData("123")]          
        [InlineData("abcdefghij")]   
        [InlineData("12345678901")]
        [InlineData("123456789a")]
        public void Validate_ShouldHaveError_WhenPhoneNumberIsInvalid(string invalidPhone)
        {
            var command = new RegisterAccountCommand(
                "user1", "valid@email.com", "John", "Doe", "Password123", "1234567890", invalidPhone);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        }

        [Fact]
        public void Should_NotHaveError_WhenRequestIsValid()
        {
            var command = new RegisterAccountCommand(
                "npappas", "npappas@example.com", "Nikos", "Pappas", "Password123!", "AN12345678", "6987654321");

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
