import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { AccountsComponent } from './accounts/accounts.component';
import { BalanceSheetComponent } from './balance-sheet/balance-sheet.component';
import { CreditCardsComponent } from './credit-cards/credit-cards.component';
import { ExpensesComponent } from './expenses/expenses.component';
import { HomeComponent } from './home/home.component';

@NgModule({ declarations: [
        AppComponent,
        NavMenuComponent,
        AccountsComponent,
        BalanceSheetComponent,
        CreditCardsComponent,
        ExpensesComponent,
        HomeComponent
    ],
    bootstrap: [AppComponent], imports: [BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        FormsModule,
        RouterModule.forRoot([
            { path: '', component: HomeComponent },
            { path: 'accounts', component: AccountsComponent },
            { path: 'balance-sheet', component: BalanceSheetComponent },
            { path: 'credit-cards', component: CreditCardsComponent },
            { path: 'expenses', component: ExpensesComponent },
        ], {})], providers: [provideHttpClient(withInterceptorsFromDi())] })
export class AppModule { }
