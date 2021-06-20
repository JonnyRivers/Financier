import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { ApiCreditCard } from './credit-card';

@Injectable({
  providedIn: 'root'
})
export class CreditCardService {

  private creditCardsUrl = 'api/CreditCard';

  //httpOptions = {
  //  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
  //};

  constructor(
    private http: HttpClient) { }

  /** GET heroes from the server */
  getCreditCards(): Observable<ApiCreditCard[]> {
    return this.http.get<ApiCreditCard[]>(this.creditCardsUrl)
      .pipe(
        catchError(this.handleError<ApiCreditCard[]>('getCreditCards', []))
      );
  }

  /**
   * Handle Http operation that failed.
   * Let the app continue.
   * @param operation - name of the operation that failed
   * @param result - optional value to return as the observable result
   */
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {

      // TODO: send the error to remote logging infrastructure
      console.error(error); // log to console instead

      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }
}
