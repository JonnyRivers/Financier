﻿using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public class AccountLinkViewModel : IAccountLinkViewModel
    {
        public void Setup(Account account)
        {
            AccountId = account.AccountId;
            Name = account.Name;
            Type = account.Type;
        }

        public int AccountId { get; private set; }
        public string Name { get; private set; }
        public AccountType Type { get; private set; }
    }
}
