import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-balance-sheet',
  templateUrl: './balance-sheet.component.html',
  styleUrls: ['./balance-sheet.component.css']
})
export class BalanceSheetComponent {
    public balanceSheet: BalanceSheet;

    constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
        http.get<BalanceSheet>(baseUrl + 'api/BalanceSheet').subscribe(result => {
          this.balanceSheet = result;

          var oldAccountThreshold = new Date();
          oldAccountThreshold.setDate(oldAccountThreshold.getDate() - 90);

          this.balanceSheet.assets = this.balanceSheet.assets.filter((a) => a.balance != 0 || new Date(a.lastTransactionAt) >= oldAccountThreshold)
          this.balanceSheet.assets.sort((a, b) => b.balance - a.balance);
          this.balanceSheet.liabilities = this.balanceSheet.liabilities.filter((a) => a.balance != 0 || new Date(a.lastTransactionAt) >= oldAccountThreshold)
          this.balanceSheet.liabilities.sort((a, b) => b.balance - a.balance);
        }, error => console.error(error));
    }
}

interface BalanceSheetItem {
    name: string;
    balance: number;
    lastTransactionAt: Date;
}

interface BalanceSheet {
    currencySymbol: string;
    assets: BalanceSheetItem[];
    liabilities: BalanceSheetItem[];
    totalAssets: number;
    totalLiabilities: number;
    netWorth: number;
}
