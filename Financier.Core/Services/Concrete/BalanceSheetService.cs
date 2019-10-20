﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Services
{
    public class BalanceSheetService : IBalanceSheetService
    {
        ILogger<BalanceSheetService> m_logger;
        IAccountService m_accountService;
        ICurrencyService m_currencyService;

        public BalanceSheetService(
            ILoggerFactory loggerFactory,
            IAccountService accountService, 
            ICurrencyService currencyService)
        {
            m_logger = loggerFactory.CreateLogger<BalanceSheetService>();
            m_accountService = accountService;
            m_currencyService = currencyService;
        }

        public BalanceSheet Generate(DateTime at)
        {
            Currency primaryCurrency = m_currencyService.GetPrimary();

            List<Account> accounts = m_accountService.GetAllPhysical().ToList();

            var assets = new List<BalanceSheetItem>();
            var liabilities = new List<BalanceSheetItem>();
            foreach (Account account in accounts)
            {
                var item = new BalanceSheetItem(
                    account.Name, 
                    m_accountService.GetBalanceAt(account.AccountId, at, true)
                );

                if(account.Type == AccountType.Asset)
                {
                    assets.Add(item);
                }
                else if (account.Type == AccountType.Liability)
                {
                    liabilities.Add(item);
                }
            }

            return new BalanceSheet(primaryCurrency.Symbol, assets, liabilities);
        }
    }
}
