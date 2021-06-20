import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { CreditCardSummaryListComponent } from './credit-card-summary-list/credit-card-summary-list.component';
import { BalanceSheetComponent } from './balance-sheet/balance-sheet.component';
import { ExpensesComponent } from './expenses/expenses.component';
import { HomeComponent } from './home/home.component';
import { CreditCardSummaryComponent } from './credit-card-summary/credit-card-summary.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    CreditCardSummaryListComponent,
    BalanceSheetComponent,
    ExpensesComponent,
    HomeComponent,
    CreditCardSummaryComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
    { path: '', component: HomeComponent },
    { path: 'balance-sheet', component: BalanceSheetComponent },
    { path: 'credit-card-summary-list', component: CreditCardSummaryListComponent },
    { path: 'expenses', component: ExpensesComponent },
], { relativeLinkResolution: 'legacy' })
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
