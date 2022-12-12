import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';

import { BalanceSheetComponent } from './balance-sheet/balance-sheet.component';
import { CreditCardComponent } from './credit-card/credit-card.component';
import { CreditCardsComponent } from './credit-cards/credit-cards.component';
import { ExpensesComponent } from './expenses/expenses.component';


@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    CreditCardsComponent,
    BalanceSheetComponent,
    ExpensesComponent,
    HomeComponent,
    CreditCardComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
    { path: '', component: HomeComponent },
    { path: 'balance-sheet', component: BalanceSheetComponent },
    { path: 'credit-card/:id', component: CreditCardComponent },
    { path: 'credit-cards', component: CreditCardsComponent },
    { path: 'expenses', component: ExpensesComponent },
], {})
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
