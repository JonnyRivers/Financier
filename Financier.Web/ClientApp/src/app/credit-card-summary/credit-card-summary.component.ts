import { Component, Input, OnInit } from '@angular/core';

import { ApiCreditCard, ApiCreditCardTransaction } from '../credit-card'

@Component({
  selector: 'app-credit-card-summary',
  templateUrl: './credit-card-summary.component.html',
  styleUrls: ['./credit-card-summary.component.css']
})
export class CreditCardSummaryComponent implements OnInit {
  @Input() apiCreditCard: ApiCreditCard | undefined

  constructor() { }

  ngOnInit(): void {
    this.id = this.apiCreditCard.id;
    this.name = this.apiCreditCard.name;
    this.balance = this.apiCreditCard.balance;
    this.transactions = this.apiCreditCard.transactions;

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
