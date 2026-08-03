using AuctionsSystem.AccountService.Application.Features.LoginAccount;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Tests.Features.LoginAccount
{
    public class LoginAccountCommandValidatorTests
    {
        private readonly LoginAccountCommandValidator _validator = new();

        [Fact]
        public void Validate_ShouldHaveError_WhenEmailIsEmpty()
        {
            var command = new LoginAccountCommand(
                "", "Password123!", "192.168.1.1");

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenEmailIsInvalid()
        {
            var command = new LoginAccountCommand(
                "invalid-email", "Password123!", "192.168.1.1");

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenPasswordIsEmpty()
        {
            var command = new LoginAccountCommand(
                "user@example.com", "", "192.168.1.1");

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenIpAddressIsEmpty()
        {
            var command = new LoginAccountCommand(
                "user@example.com", "Password123!", "");

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.IpAddress);
        }

        [Fact]
        public void Validate_ShouldHaveError_WhenIpAddressIsUnknown()
        {
            var command = new LoginAccountCommand(
                "user@example.com", "Password123!", "unknown");

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.IpAddress);
        }

        [Fact]
        public void Should_NotHaveError_WhenRequestIsValid()
        {
            var command = new LoginAccountCommand(
                "user@example.com", "Password123!", "192.168.1.1");

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
