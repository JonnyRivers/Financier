import { Component, OnInit } from '@angular/core';

import { ApiCreditCard } from '../credit-card'
import { CreditCardService } from '../credit-card.service'

@Component({
  selector: 'app-credit-card-summary-list',
  templateUrl: './credit-card-summary-list.component.html',
  styleUrls: ['./credit-card-summary-list.component.css']
})
export class CreditCardSummaryListComponent implements OnInit {
  apiCreditCards: ApiCreditCard[] = [];

  constructor(private creditCardService: CreditCardService) { }

  ngOnInit() {
    this.getCreditCards();
  }

  getCreditCards(): void {
    this.creditCardService.getCreditCards()
      .subscribe(apiCreditCards => this.apiCreditCards = apiCreditCards)
  }
}
