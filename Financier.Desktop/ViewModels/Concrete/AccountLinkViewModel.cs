﻿using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public class AccountLinkViewModel : IAccountLinkViewModel
    {
        public void Setup(AccountLink accountLink)
        {
            AccountId = accountLink.AccountId;
            Name = accountLink.Name;
            Type = accountLink.Type;
        }

        public AccountLink ToAccountLink()
        {
            return new AccountLink
            {
                AccountId = AccountId,
                Name = Name,
                Type = Type
            };
        }

        public int AccountId { get; private set; }
        public string Name { get; private set; }
        public AccountType Type { get; private set; }
    }
}