import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiConfig } from '../../environments/api.config';
import { InteractionType } from '../enums/interactionType';
import Interaction from '../models/interaction';

@Injectable({
  providedIn: 'root',
})
export class InteractionsService {
  private basePath: string = ApiConfig.interactions;

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
