import { Injectable } from '@angular/core';
import { Observable, catchError, map } from 'rxjs';
import { HttpService } from './http.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private _apiToken: string;
  private _userId: string;

  constructor(private httpService: HttpService) {
    this._apiToken = '';
    this._userId = '';
   }

  login(userName: string): Observable<any> {
    return this.httpService.post('http://localhost:5208/auth/login', {
      "userName": userName
    }).pipe(
        map(response => {
          this._apiToken = response['token'];
          this._userId = response['userId'];
    }));
  }

  public get apiToken(): string {
    return this._apiToken;
  }

  public get userId(): string {
    return this._userId;
  }
}
