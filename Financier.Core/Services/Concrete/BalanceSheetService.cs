using Microsoft.Extensions.Logging;
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
            List<AccountExtended> accountsExtended = m_accountService.GetExtended(accounts, at).ToList();

            var assets = new List<BalanceSheetItem>();
            var liabilities = new List<BalanceSheetItem>();
            foreach (AccountExtended accountExtended in accountsExtended)
            {
                if (!accountExtended.LastTransactionAt.HasValue)
                    continue;

                var item = new BalanceSheetItem(
                    accountExtended.Name,
                    accountExtended.Balance,
                    accountExtended.LastTransactionAt.Value
                );

                if(accountExtended.Type == AccountType.Asset)
                {
                    assets.Add(item);
                }
                else if (accountExtended.Type == AccountType.Liability)
                {
                    liabilities.Add(item);
                }
            }

            return new BalanceSheet(primaryCurrency.Symbol, assets, liabilities);
        }
    }
}
