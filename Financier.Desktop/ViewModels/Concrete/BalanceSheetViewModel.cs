using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Financier.Desktop.ViewModels
{
    public class BalanceSheetViewModelFactory : IBalanceSheetViewModelFactory
    {
        private readonly ILogger<BalanceSheetViewModelFactory> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public BalanceSheetViewModelFactory(ILogger<BalanceSheetViewModelFactory> logger, IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        public IBalanceSheetViewModel Create()
        {
            return m_serviceProvider.CreateInstance<BalanceSheetViewModel>();
        }
    }

    public class BalanceSheetViewModel : BaseViewModel, IBalanceSheetViewModel
    {
        private ILogger<BalanceSheetViewModel> m_logger;
        private IBalanceSheetService m_balanceSheetService;
        private IViewModelFactory m_viewModelFactory;

        private DateTime m_at;
        private ObservableCollection<IBalanceSheetItemViewModel> m_assets;
        private decimal m_totalAssets;
        private ObservableCollection<IBalanceSheetItemViewModel> m_liabilities;
        private decimal m_totalLiabilities;
        private decimal m_netWorth;

        public BalanceSheetViewModel(
            ILogger<BalanceSheetViewModel> logger,
            IBalanceSheetService balanceSheetService,
            IViewModelFactory viewModelFactory)
        {
            m_logger = logger;
            m_balanceSheetService = balanceSheetService;
            m_viewModelFactory = viewModelFactory;

            m_at = DateTime.Now;

            Load();
        }

        // This should be async
        private void Load()
        {
            BalanceSheet balanceSheet = m_balanceSheetService.Generate(m_at);

            Assets = new ObservableCollection<IBalanceSheetItemViewModel>(
                balanceSheet.Assets.Select(i => m_viewModelFactory.CreateBalanceSheetItemViewModel(i)));
            TotalAssets = balanceSheet.TotalAssets;
            Liabilities = new ObservableCollection<IBalanceSheetItemViewModel>(
                balanceSheet.Liabilities.Select(i => m_viewModelFactory.CreateBalanceSheetItemViewModel(i)));
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
