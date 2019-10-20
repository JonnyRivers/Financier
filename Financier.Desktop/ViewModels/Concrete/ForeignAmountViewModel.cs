using Financier.Services;
using Microsoft.Extensions.Logging;
using System;

namespace Financier.Desktop.ViewModels
{
    public class ForeignAmountViewModelFactory : IForeignAmountViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;
        private ICurrencyExchangeService m_currencyExchangeService;

        public ForeignAmountViewModelFactory(
            ILoggerFactory loggerFactory,
            ICurrencyExchangeService currencyExchangeService)
        {
            m_loggerFactory = loggerFactory;
            m_currencyExchangeService = currencyExchangeService;
        }

        public IForeignAmountViewModel Create(
            decimal nativeAmount,
            string nativeCurrencyCode,
            string foreignCurrencyCode)
        {
            return new ForeignAmountViewModel(
                m_loggerFactory,
                m_currencyExchangeService,
                nativeAmount, 
                nativeCurrencyCode, 
                foreignCurrencyCode);
        }
    }

    public class ForeignAmountViewModel : BaseViewModel, IForeignAmountViewModel
    {
        private ILogger<ForeignAmountViewModel> m_logger;
        private ICurrencyExchangeService m_currencyExchangeService;

        private decimal m_foreignAmount;
        private string m_foreignCurrencyCode;
        private decimal m_foreignToNativeRate;
        private decimal m_nativeAmount;
        private string m_nativeCurrencyCode;

        public ForeignAmountViewModel(
            ILoggerFactory loggerFactory,
            ICurrencyExchangeService currencyExchangeService,
            decimal nativeAmount,
            string nativeCurrencyCode,
            string foreignCurrencyCode)
        {
            m_logger = loggerFactory.CreateLogger<ForeignAmountViewModel>();
            m_currencyExchangeService = currencyExchangeService;

            m_foreignCurrencyCode = foreignCurrencyCode;
            m_nativeCurrencyCode = nativeCurrencyCode;
            m_foreignToNativeRate = m_currencyExchangeService.GetExchangeRate(
                m_foreignCurrencyCode,
                m_nativeCurrencyCode,
                DateTime.Now
            );
            m_nativeAmount = nativeAmount;
            m_foreignAmount = Math.Round((m_nativeAmount / m_foreignToNativeRate), 2);
        }

        public decimal ForeignAmount
        {
            get
            {
                return m_foreignAmount;
            }
            set
            {
                if (m_foreignAmount != value)
                {
                    m_foreignAmount = value;

                    OnPropertyChanged();

                    NativeAmount = Math.Round((m_foreignAmount * m_foreignToNativeRate), 2);
                }
            }
        }

        public string ForeignCurrencyCode => m_foreignCurrencyCode;

        public decimal ForeignToNativeRate => m_foreignToNativeRate;

        public decimal NativeAmount
        {
            get
            {
                return m_nativeAmount;
            }
            set
            {
                if (m_nativeAmount != value)
                {
                    m_nativeAmount = value;

                    OnPropertyChanged();
                }
            }
        }

        public string NativeCurrencyCode => m_nativeCurrencyCode;
    }
}
