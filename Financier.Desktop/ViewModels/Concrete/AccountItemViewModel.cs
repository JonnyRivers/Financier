﻿using Financier.Services;
using Microsoft.Extensions.Logging;
using System;

namespace Financier.Desktop.ViewModels
{
    public class AccountItemViewModelFactory : IAccountItemViewModelFactory
    {
        private readonly ILogger<AccountItemViewModelFactory> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public AccountItemViewModelFactory(ILogger<AccountItemViewModelFactory> logger, IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        public IAccountItemViewModel Create(Account account)
        {
            return m_serviceProvider.CreateInstance<AccountItemViewModel>(account);
        }
    }

    public class AccountItemViewModel : BaseViewModel, IAccountItemViewModel
    {
        private ILogger<AccountItemViewModel> m_logger;

        private string m_name;
        private AccountType m_type;
        private AccountSubType m_subType;
        private string m_currencyName;

        public AccountItemViewModel(
            ILogger<AccountItemViewModel> logger,
            Account account)
        {
            m_logger = logger;

            AccountId = account.AccountId;
            m_name = account.Name;
            m_type = account.Type;
            m_subType = account.SubType;
            m_currencyName = account.Currency.Name;
        }

        public int AccountId { get; private set; }
        public string Name
        {
            get { return m_name; }
            private set
            {
                if (m_name != value)
                {
                    m_name = value;

                    OnPropertyChanged();
                }
            }
        }
        public AccountType Type
        {
            get { return m_type; }
            private set
            {
                if (m_type != value)
                {
                    m_type = value;

                    OnPropertyChanged();
                }
            }
        }
        public AccountSubType SubType
        {
            get { return m_subType; }
            private set
            {
                if (m_subType != value)
                {
                    m_subType = value;

                    OnPropertyChanged();
                }
            }
        }
        public string CurrencyName
        {
            get { return m_currencyName; }
            private set
            {
                if (m_currencyName != value)
                {
                    m_currencyName = value;

                    OnPropertyChanged();
                }
            }
        }
    }
}
