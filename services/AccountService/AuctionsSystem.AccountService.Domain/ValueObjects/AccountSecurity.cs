using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Domain.ValueObjects
{
    public class AccountSecurity
    {
        public string PasswordHash { get; private set; }
        public bool TwoFactorEnabled {  get; private set; }
        public int AccessFailedCount {  get; private set; }
        public DateTimeOffset? LockoutEnd {  get; private set; }

        protected AccountSecurity() { }

        private AccountSecurity(string passwordHash)
        {
            PasswordHash = passwordHash;
            TwoFactorEnabled = false;
            AccessFailedCount = 0;
        }

        public void RecordLoginSuccess()
        {
            AccessFailedCount = 0;
            LockoutEnd = null;
        }

        public void RecordLoginFailure(int maxFailedAttempts, TimeSpan lockoutPeriod)
        {
            AccessFailedCount++;
            if (AccessFailedCount >= maxFailedAttempts)
            {
                LockoutEnd = DateTimeOffset.UtcNow.Add(lockoutPeriod);
            }
        }

        public void RecordUnsuccesfulLoginTry()
        {
            AccessFailedCount++;
        }


        public bool IsLockedOut()
        {
            return false;
        }


        public static AccountSecurity CreateIntialSecurity(string passwordHash)
        {
            return new AccountSecurity(passwordHash);
        }
    }
}
