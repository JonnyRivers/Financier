using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Services
{
    // TODO: replace this with something implemented in terms of the data services
    public class BalanceSheetService : IBalanceSheetService
    {
        ILogger<BalanceSheetService> m_logger;
        IAccountService m_accountService;
        ICurrencyService m_currencyService;

        public BalanceSheetService(
            ILogger<BalanceSheetService> logger, 
            IAccountService accountService, 
            ICurrencyService currencyService)
        {
            m_logger = logger;
            m_accountService = accountService;
            m_currencyService = currencyService;
        }

        public BalanceSheet Generate(DateTime at)
        {
            // TODO: this is implemented in terms of very low level (data layer) concepts.
            // This should be implemented in terms of other services.

            Currency primaryCurrency = m_currencyService.GetPrimary();

            List<Account> accounts = m_accountService.GetAllPhysical().ToList();

            var assets = new List<BalanceSheetItem>();
            var liabilities = new List<BalanceSheetItem>();
            foreach (Account account in accounts)
            {
                // TODO: we are hitting the database too much
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
