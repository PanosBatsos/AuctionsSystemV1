using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Domain.ValueObjects
{
    public class AccountVerification
    {
        public bool EmailConfirmed { get; private set; }
        public bool PhoneNumberConfirmed{  get; private set; }

        private AccountVerification()
        {
            EmailConfirmed = false;
            PhoneNumberConfirmed = false;
        }

        public void ConfirmEmail()
        {
            EmailConfirmed = true;
        }

        public void ConfirmPhoneNumber()
        {
            PhoneNumberConfirmed = true;
        }

        public void RevokePhoneConfirmation()
        {
            PhoneNumberConfirmed = false;
        }

        public void RevokeEmailConfirmation()
        {
            EmailConfirmed = false;
        }

        public bool IsVerified()
        {
            return EmailConfirmed && PhoneNumberConfirmed;
        }

        public static AccountVerification NoVerification()
        {
            return new AccountVerification();
        }
    }
}
