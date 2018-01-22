﻿using System.Collections.ObjectModel;
using System.Linq;
using Financier.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class BudgetTransactionItemViewModel : BaseViewModel, IBudgetTransactionItemViewModel
    {
        private ILogger<BudgetTransactionItemViewModel> m_logger;

        public BudgetTransactionItemViewModel(ILogger<BudgetTransactionItemViewModel> logger, IAccountService accountService)
        {
            m_logger = logger;
        }

        public void Setup(
            ObservableCollection<IAccountLinkViewModel> accountLinks,
            BudgetTransaction budgetTransaction, 
            BudgetTransactionType type)
        {
            AccountLinks = accountLinks;
            Amount = budgetTransaction.Amount;
            BudgetTransactionId = budgetTransaction.BudgetTransactionId;
            SelectedCreditAccount = AccountLinks.Single(al => al.AccountId == budgetTransaction.CreditAccount.AccountId);
            SelectedDebitAccount = AccountLinks.Single(al => al.AccountId == budgetTransaction.DebitAccount.AccountId);
            Type = type;
        }

        public BudgetTransaction ToBudgetTransaction()
        {
            var budgetTransaction = new BudgetTransaction
            {
                BudgetTransactionId = BudgetTransactionId,
                CreditAccount = SelectedCreditAccount.ToAccountLink(),
                DebitAccount = SelectedDebitAccount.ToAccountLink(),
                Amount = Amount
            };

            return budgetTransaction;
        }

        public int BudgetTransactionId { get; set; }
        public BudgetTransactionType Type { get; set; }
        public IAccountLinkViewModel SelectedCreditAccount { get; set; }
        public IAccountLinkViewModel SelectedDebitAccount { get; set; }
        public decimal Amount { get; set; }

        public ObservableCollection<IAccountLinkViewModel> AccountLinks { get; private set; }
    }
}