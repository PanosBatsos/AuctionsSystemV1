using AuctionsSystem.AccountService.Domain.Enums;
using AuctionsSystem.AccountService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Domain.Entities
{
    public class Account
    {
        public Guid Id { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string PhoneNumber { get; private set; }
        public string IdNumber { get; private set; }
        public UserRole Role { get; private set; }
        public AccountSecurity Security { get; private set; }
        public AccountVerification Verification { get; private set; }
        public bool IsActive { get; private set; }
        public DateTimeOffset TermsAcceptedAt { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? UpdatedAt { get; private set; }
        public DateTimeOffset? LastLoginAt { get; private set; }
        public string? LastLoginIp { get; private set; }

        protected Account() { }

        public Account(string userName, string email, string passwordHash, string firstName, string lastName, string idNumber, string phoneNumber, UserRole role, DateTimeOffset termsAcceptedAt)
        {
            Id = Guid.NewGuid();
            UserName = userName;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            IdNumber = idNumber;
            Role = role;
            PhoneNumber = phoneNumber;
            TermsAcceptedAt = termsAcceptedAt;
            IsActive = true;
            CreatedAt = DateTimeOffset.UtcNow;
            Security = new AccountSecurity(passwordHash);
            Verification = new AccountVerification();
        }

        public void ChangeUsername(string userName)
        {
            UserName = userName;
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void ChangeEmail(string email)
        {
            Email = email;

            Verification.RevokeEmailConfirmation();
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void ChangePhoneNumber(string phoneNumber)
        {
            PhoneNumber = phoneNumber;

            Verification.RevokePhoneConfirmation();
            UpdatedAt = DateTimeOffset.UtcNow;
        }


        public void DeactivateAccount()
        {
            IsActive = false;
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void ConfirmEmail()
        {
            Verification.ConfirmEmail();
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void ConfirmPhoneNumber()
        {
            Verification.ConfirmPhoneNumber();
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void RecordLoginSuccess(string ipAddress)
        {
            LastLoginAt = DateTimeOffset.UtcNow;
            LastLoginIp = ipAddress;

            Security.RecordLoginSuccess();

            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void RecordLoginFailure(int maxFailedAttempts, TimeSpan lockoutPeriod)
        {

            Security.RecordLoginFailure(maxFailedAttempts, lockoutPeriod);

            UpdatedAt = DateTimeOffset.UtcNow;
        }
    }
}
