﻿using Financier.Desktop.Commands;
using Financier.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class AccountEditViewModel : IAccountEditViewModel
    {
        public AccountEditViewModel(IAccountService accountService, ICurrencyService currencyService)
        {
            m_accountService = accountService;
            m_currencyService = currencyService;
            m_accountId = 0;

            AccountTypes = Enum.GetValues(typeof(Entities.AccountType)).Cast<Entities.AccountType>();
            Currencies = m_currencyService.GetAll();

            Name = "New Account";
            SelectedAccountType = Entities.AccountType.Asset;
            SelectedCurrency = m_currencyService.GetPrimary();
        }

        private IAccountService m_accountService;
        private ICurrencyService m_currencyService;
        private int m_accountId;

        public IEnumerable<Entities.AccountType> AccountTypes { get; }
        public IEnumerable<Currency> Currencies { get; }

        public int AccountId
        {
            get { return m_accountId; }
            set
            {
                if(value != m_accountId)
                {
                    m_accountId = value;

                    Entities.Account account = m_accountService.Get(m_accountId);
                    Name = account.Name;
                    SelectedAccountType = account.Type;
                    SelectedCurrency = account.Currency;
                }
            }
        }

        public string Name { get; set; }
        public Entities.AccountType SelectedAccountType { get; set; }
        public Currency SelectedCurrency { get; set; }

        public ICommand OKCommand => new RelayCommand(OKExecute);
        public ICommand CancelCommand => new RelayCommand(CancelExecute);

        private void OKExecute(object obj)
        {
            if (m_accountId != 0)
            {
                Entities.Account account = m_accountService.Get(m_accountId);
                account.Name = Name;
                account.Type = SelectedAccountType;
                account.CurrencyId = SelectedCurrency.CurrencyId;

                m_accountService.Update(account);
            }
            else
            {
                var account = new Entities.Account
                {
                    Name = Name,
                    Type = SelectedAccountType,
                    CurrencyId = SelectedCurrency.CurrencyId
                };

                m_accountService.Create(account);
            }
        }

        private void CancelExecute(object obj)
        {

        }
    }
}
