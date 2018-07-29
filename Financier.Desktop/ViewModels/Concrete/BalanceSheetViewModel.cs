using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Financier.Desktop.ViewModels
{
    public class BalanceSheetViewModel : BaseViewModel, IBalanceSheetViewModel
    {
        private ILogger<BalanceSheetViewModel> m_logger;
        private IBalanceSheetService m_balanceSheetService;
        private IViewModelFactory m_viewModelFactory;

        public BalanceSheetViewModel(
            ILogger<BalanceSheetViewModel> logger,
            IBalanceSheetService balanceSheetService,
            IViewModelFactory viewModelFactory)
        {
            m_logger = logger;
            m_balanceSheetService = balanceSheetService;
            m_viewModelFactory = viewModelFactory;

            BalanceSheet balanceSheet = m_balanceSheetService.Generate(DateTime.Now);

            Assets = new ObservableCollection<IBalanceSheetItemViewModel>(
                balanceSheet.Assets.Select(i => m_viewModelFactory.CreateBalanceSheetItemViewModel(i)));
            TotalAssets = balanceSheet.TotalAssets;
            Liabilities = new ObservableCollection<IBalanceSheetItemViewModel>(
                balanceSheet.Liabilities.Select(i => m_viewModelFactory.CreateBalanceSheetItemViewModel(i)));
            TotalLiabilities = balanceSheet.TotalLiabilities;
            NetWorth = balanceSheet.NetWorth;
        }

        public ObservableCollection<IBalanceSheetItemViewModel> Assets { get; }
        public decimal TotalAssets { get; }
        public ObservableCollection<IBalanceSheetItemViewModel> Liabilities { get; }
        public decimal TotalLiabilities { get; }
        public decimal NetWorth { get; }
    }
}
