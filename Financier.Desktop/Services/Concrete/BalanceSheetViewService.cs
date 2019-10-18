﻿using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class BalanceSheetViewService : IBalanceSheetViewService
    {
        private readonly ILogger<BalanceSheetViewService> m_logger;
        private readonly IBalanceSheetViewModelFactory m_balanceSheetViewModelFactory;

        public BalanceSheetViewService(ILogger<BalanceSheetViewService> logger, IBalanceSheetViewModelFactory balanceSheetViewModelFactory)
        {
            m_logger = logger;
            m_balanceSheetViewModelFactory = balanceSheetViewModelFactory;
        }

        public void Show()
        {
            IBalanceSheetViewModel viewModel = m_balanceSheetViewModelFactory.Create();
            var window = new BalanceSheetWindow(viewModel);
            window.Show();
        }
    }
}
