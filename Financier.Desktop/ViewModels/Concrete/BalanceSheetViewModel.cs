using Financier.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Financier.Desktop.ViewModels
{
    public class BalanceSheetViewModelFactory : IBalanceSheetViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;
        private readonly IBalanceSheetService m_balanceSheetService;
        private readonly IBalanceSheetItemViewModelFactory m_balanceSheetItemViewModelFactory;

        public BalanceSheetViewModelFactory(
            ILoggerFactory loggerFactory,
            IBalanceSheetService balanceSheetService,
            IBalanceSheetItemViewModelFactory balanceSheetItemViewModelFactory)
        {
            m_loggerFactory = loggerFactory;
            m_balanceSheetService = balanceSheetService;
            m_balanceSheetItemViewModelFactory = balanceSheetItemViewModelFactory;
        }

        public IBalanceSheetViewModel Create()
        {
            return new BalanceSheetViewModel(
                m_loggerFactory,
                m_balanceSheetService,
                m_balanceSheetItemViewModelFactory);
        }
    }

    public class BalanceSheetViewModel : BaseViewModel, IBalanceSheetViewModel
    {
        private readonly ILogger<BalanceSheetViewModel> m_logger;
        private readonly IBalanceSheetService m_balanceSheetService;
        private readonly IBalanceSheetItemViewModelFactory m_balanceSheetItemViewModelFactory;

        private DateTime m_at;
        private ObservableCollection<IBalanceSheetItemViewModel> m_assets;
        private decimal m_totalAssets;
        private ObservableCollection<IBalanceSheetItemViewModel> m_liabilities;
        private decimal m_totalLiabilities;
        private decimal m_netWorth;

        public BalanceSheetViewModel(
            ILoggerFactory loggerFactory,
            IBalanceSheetService balanceSheetService,
            IBalanceSheetItemViewModelFactory balanceSheetItemViewModelFactory)
        {
            m_logger = loggerFactory.CreateLogger<BalanceSheetViewModel>();
            m_balanceSheetService = balanceSheetService;
            m_balanceSheetItemViewModelFactory = balanceSheetItemViewModelFactory;

            m_at = DateTime.Now;

            Load();
        }

        // This should be async
        private void Load()
        {
            BalanceSheet balanceSheet = m_balanceSheetService.Generate(m_at);

            Assets = new ObservableCollection<IBalanceSheetItemViewModel>(
                balanceSheet.Assets.Select(i => m_balanceSheetItemViewModelFactory.Create(i)));
            TotalAssets = balanceSheet.TotalAssets;
            Liabilities = new ObservableCollection<IBalanceSheetItemViewModel>(
                balanceSheet.Liabilities.Select(i => m_balanceSheetItemViewModelFactory.Create(i)));
            TotalLiabilities = balanceSheet.TotalLiabilities;
            NetWorth = balanceSheet.NetWorth;
        }

        public DateTime At
        {
            get
            {
                return m_at;
            }
            set
            {
                if(m_at != value)
                {
                    m_at = value;
                    OnPropertyChanged();

                    Load();
                }
            }
        }

        public ObservableCollection<IBalanceSheetItemViewModel> Assets
        {
            get
            {
                return m_assets;
            }
            set
            {
                if (m_assets != value)
                {
                    m_assets = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal TotalAssets
        {
            get
            {
                return m_totalAssets;
            }
            set
            {
                if (m_totalAssets != value)
                {
                    m_totalAssets = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<IBalanceSheetItemViewModel> Liabilities
        {
            get
            {
                return m_liabilities;
            }
            set
            {
                if (m_liabilities != value)
                {
                    m_liabilities = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal TotalLiabilities
        {
            get
            {
                return m_totalLiabilities;
            }
            set
            {
                if (m_totalLiabilities != value)
                {
                    m_totalLiabilities = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal NetWorth
        {
            get
            {
                return m_netWorth;
            }
            set
            {
                if (m_netWorth != value)
                {
                    m_netWorth = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
