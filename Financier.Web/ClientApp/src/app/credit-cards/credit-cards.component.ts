import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-credit-cards',
  templateUrl: './credit-cards.component.html',
  styleUrls: ['./credit-cards.component.css']
})
export class CreditCardsComponent {
  public creditCards: CreditCard[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<ApiCreditCard[]>(baseUrl + 'api/CreditCard').subscribe(result => {
      let creditCards: CreditCard[] = new Array();
      for (let apiCreditCard of result) {
        creditCards.push(new CreditCard(apiCreditCard));
      }
      this.creditCards = creditCards.sort((a, b) => a.name > b.name ? 1 : -1);
    }, error => console.error(error));
  }
}

class CreditCard {
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

interface ApiCreditCardTransaction {
  id: number;
  at: Date;
  otherAccountName: string;
  amount: number;
  runningBalance: number;
}

interface ApiCreditCard {
  id: number;
  name: string;
  balance: number;
  transactions: ApiCreditCardTransaction[];
}
