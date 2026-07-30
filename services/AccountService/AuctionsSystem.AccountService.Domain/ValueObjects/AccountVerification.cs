using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Domain.ValueObjects
{
    public class AccountVerification
    {
        public bool EmailConfirmed { get; private set; }
        public bool PhoneNumberConfirmed{  get; private set; }

        public AccountVerification()
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
    }
}
