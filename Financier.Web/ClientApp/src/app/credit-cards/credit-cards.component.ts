import { Component, OnInit } from '@angular/core';

import { ApiCreditCard } from '../credit-card'
import { CreditCardService } from '../credit-card.service'

@Component({
  selector: 'app-credit-cards',
  templateUrl: './credit-cards.component.html',
  styleUrls: ['./credit-cards.component.css']
})
export class CreditCardsComponent implements OnInit {
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
