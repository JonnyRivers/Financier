import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-expenses',
  templateUrl: './expenses.component.html',
  styleUrls: ['./expenses.component.css']
})
export class ExpensesComponent {
    public expenseAccounts: ExpenseAccount[];

    constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
        http.get<ExpenseAccount[]>(baseUrl + 'api/expenses').subscribe(result => {
            this.expenseAccounts = result.sort((a, b) => (a.name > b.name ? 1 : -1));
        }, error => console.error(error));
    }
}

interface ExpenseAccount {
    id: number;
    name: string;
    balance: number;
    recentTransactions: ExpenseTransaction[];
}

interface ExpenseTransaction {
    id: number;
    at: Date;
    otherAccountName: string;
    amount: number;
    balance: number;
}
