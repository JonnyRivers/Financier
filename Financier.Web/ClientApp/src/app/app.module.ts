import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { CreditCardsComponent } from './credit-cards/credit-cards.component';
import { BalanceSheetComponent } from './balance-sheet/balance-sheet.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    CreditCardsComponent,
    BalanceSheetComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: CreditCardsComponent, pathMatch: 'full' },
      { path: 'balance-sheet', component: BalanceSheetComponent },
      { path: 'credit-cards', component: CreditCardsComponent },
    ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
