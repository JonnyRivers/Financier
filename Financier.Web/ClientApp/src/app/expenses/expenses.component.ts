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
        http.get<ApiExpenseAccount[]>(baseUrl + 'api/expenses').subscribe(result => {
            let accounts: ExpenseAccount[] = new Array();
            for (let apiAccount of result) {
                accounts.push(new ExpenseAccount(apiAccount));
            }
            this.expenseAccounts = accounts.sort((a, b) => (a.name > b.name ? 1 : -1));
        }, error => console.error(error));
    }
}

enum TransactionsVisibility {
    Hidden,
    Recent,
    All
}

class ExpenseAccount {
    constructor(apiAccount: ApiExpenseAccount) {
        this.id = apiAccount.id;
        this.name = apiAccount.name;
        this.balance = apiAccount.balance;
        this.transactions = apiAccount.transactions;

        this.transactionsVisibility = TransactionsVisibility.Hidden;
        this.toggleHint = "(show recent transactions)"
        this.visibleTransactions = new Array();
        let recentTransactionCutoff = new Date();
        recentTransactionCutoff.setDate(recentTransactionCutoff.getDate() - 14);
        this.recentTransactions = this.transactions.filter(t => new Date(t.at) > recentTransactionCutoff);
    }

    public toggleVisibility() {
        if (this.transactionsVisibility == TransactionsVisibility.Hidden) {
            this.toggleHint = "(show all transactions)"
            this.transactionsVisibility = TransactionsVisibility.Recent;
            this.visibleTransactions = this.recentTransactions;
        }
        else if (this.transactionsVisibility == TransactionsVisibility.Recent) {
            this.toggleHint = "(hide all transactions)"
            this.transactionsVisibility = TransactionsVisibility.All;
            this.visibleTransactions = this.transactions;
        }
        else if (this.transactionsVisibility == TransactionsVisibility.All) {
            this.toggleHint = "(show recent transactions)"
            this.transactionsVisibility = TransactionsVisibility.Hidden;
            this.visibleTransactions = new Array();
        }
    }

    public id: number;
    public name: string;
    public balance: number;
    public transactions: ApiExpenseTransaction[];

    private transactionsVisibility: TransactionsVisibility;
    public toggleHint: string;
    private recentTransactions: ApiExpenseTransaction[];
    private visibleTransactions: ApiExpenseTransaction[];
}

interface ApiExpenseAccount {
    id: number;
    name: string;
    balance: number;
    transactions: ApiExpenseTransaction[];
}

interface ApiExpenseTransaction {
    id: number;
    at: Date;
    otherAccountName: string;
    amount: number;
    balance: number;
}
