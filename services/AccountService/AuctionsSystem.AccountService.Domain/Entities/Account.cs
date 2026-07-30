using AuctionsSystem.AccountService.Domain.Enums;
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
        public string PasswordHash { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string IdNumber { get; private set; }
        public UserRole Role { get; private set; }
        public bool IsActive { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset? UpdatedAt { get; private set; }

        protected Account() { }

        public Account(string userName, string email, string passwordHash, string firstName, string lastName, string idNumber, UserRole role)
        {
            Id = Guid.NewGuid();
            Email = email;
            UserName = userName;
            PasswordHash = passwordHash;
            FirstName = firstName;
            LastName = lastName;
            IdNumber = idNumber;
            Role = role;
            IsActive = true;
            CreatedAt = DateTimeOffset.UtcNow;
        }


        public void ChangeUsername(string userName)
        {
            UserName = userName;
            UpdatedAt = DateTimeOffset.UtcNow;
        }

        public void DeactivateAccount()
        {
            IsActive = false;
            UpdatedAt = DateTimeOffset.UtcNow;
        }
    }
}
