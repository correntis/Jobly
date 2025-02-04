import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiConfig } from '../../environments/api.config';
import Currency from '../models/currency';

@Injectable({
  providedIn: 'root',
})
export class CurrenciesService {
  private basePath: string = ApiConfig.currencies;

  constructor(private httpClient: HttpClient) {}

  get(): Observable<Currency[]> {
    return this.httpClient.get<Currency[]>(`${this.basePath}`);
  }
}
