import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-credit-cards',
  templateUrl: './credit-cards.component.html'
})
export class CreditCardsComponent {
  public creditCards: CreditCard[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<CreditCard[]>(baseUrl + 'api/CreditCard').subscribe(result => {
      this.creditCards = result;
    }, error => console.error(error));
  }
}

interface CreditCard {
  id: number;
  name: string;
}
