export class CreditCard {
  constructor(apiCreditCard: ApiCreditCard) {
    this.id = apiCreditCard.id;
    this.name = apiCreditCard.name;
    this.balance = apiCreditCard.balance;
    this.transactions = apiCreditCard.transactions;

    this.areTransactionsVisible = false;
    this.visibleTransactions = new Array();
    let recentTransactionCutoff = new Date();
    recentTransactionCutoff.setDate(recentTransactionCutoff.getDate() - 14);
    this.recentTransactions = this.transactions.filter(t => new Date(t.at) > recentTransactionCutoff);
  }

  public toggleTransactionsVisibility() {
    this.areTransactionsVisible = !this.areTransactionsVisible;

    if (this.areTransactionsVisible) {
      this.visibleTransactions = this.recentTransactions;
    }
    else {
      this.visibleTransactions = new Array();
    }
  }

  public id: number;
  public name: string;
  public balance: number;
  public transactions: ApiCreditCardTransaction[];

  public areTransactionsVisible: boolean;
  private recentTransactions: ApiCreditCardTransaction[];
  public visibleTransactions: ApiCreditCardTransaction[];
}

export interface ApiCreditCardTransaction {
  id: number;
  at: Date;
  otherAccountName: string;
  amount: number;
  runningBalance: number;
}

export interface ApiCreditCard {
  id: number;
  name: string;
  balance: number;
  transactions: ApiCreditCardTransaction[];
}
