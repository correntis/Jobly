import { Injectable } from '@angular/core';
import { ApiConfig } from '../../environments/api.config';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import Currency from '../models/currency';

@Injectable({
  providedIn: 'root',
})
export class CurrenciesService {
  basePath = ApiConfig.currencies;

  constructor(private httpClient: HttpClient) {}

  get(): Observable<Currency[]> {
    return this.httpClient.get<Currency[]>(`${this.basePath}`);
  }
}
