import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-credit-card',
  templateUrl: './credit-card.component.html',
  styleUrls: ['./credit-card.component.css']
})
export class CreditCardComponent {
  public accountTransactions: ApiAccountTransaction[];

  constructor(
    http: HttpClient,
    @Inject('BASE_URL')
    baseUrl: string,
    route: ActivatedRoute) {
      const id = Number(route.snapshot.paramMap.get('id'));
    http.get<ApiAccountTransaction[]>(baseUrl + 'api/CreditCard/' + id + '/transactions').subscribe(result => {
        this.accountTransactions = result;
      }, error => console.error(error));
  }
}

interface ApiAccountTransaction {
  debitAccountName: string;
  creditAccountName: string;
  at: Date;
  amount: number;
  balance: number;
}
