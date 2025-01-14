import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiConfig } from '../../environments/api.config';
import { Observable } from 'rxjs';
import Interaction from '../models/interaction';
import { InteractionType } from '../enums/interactionType';

@Injectable({
  providedIn: 'root',
})
export class InteractionsService {
  basePath = ApiConfig.interactions;

  constructor(private httpClient: HttpClient) {}

  add(
    userId: string,
    vacancyId: string,
    type: InteractionType
  ): Observable<string> {
    return this.httpClient.post<string>(`${this.basePath}`, {
      userId,
      vacancyId,
      type,
    });
  }

  getByVacancy(vacancyId: string): Observable<Interaction[]> {
    return this.httpClient.get<Interaction[]>(
      `${this.basePath}/vacancies/${vacancyId}`
    );
  }

  getByUser(userId: string): Observable<Interaction[]> {
    return this.httpClient.get<Interaction[]>(
      `${this.basePath}/users/${userId}`
    );
  }
}
