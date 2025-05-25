import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-accounts',
  templateUrl: './accounts.component.html',
  styleUrls: ['./accounts.component.css']
})
export class AccountsComponent {
  public accounts: Account[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<Account[]>(baseUrl + 'api/Accounts').subscribe(result => {
      this.accounts = result;
    }, error => console.error(error));
  }
}

interface Account {
  id: number;
  name: string;
  //balance: number;
}
