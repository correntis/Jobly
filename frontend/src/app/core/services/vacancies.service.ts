import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiConfig } from '../../environments/api.config';
import { InteractionType } from '../enums/interactionType';
import Vacancy from '../models/vacancies/vacancy';
import AddVacancyDetailsRequest from '../requests/vacancies/addVacancyDetailsRequest';
import { AddVacancyRequest } from '../requests/vacancies/addVacancyRequest';
import { VacanciesFilter } from './../models/vacancies/vacanciesFilter';

@Injectable({
  providedIn: 'root',
})
export class VacanciesService {
  private basePath: string = ApiConfig.vacancies;

  constructor(private httpClient: HttpClient) {}

  add(addVacancyRequest: AddVacancyRequest): Observable<string> {
    return this.httpClient.post<string>(`${this.basePath}`, addVacancyRequest);
  }

  addDetails(addDetailsRequest: AddVacancyDetailsRequest): Observable<string> {
    return this.httpClient.post<string>(
      `${this.basePath}/details`,
      addDetailsRequest
    );
  }

  archive(id: string): Observable<string> {
    return this.httpClient.post<string>(`${this.basePath}/archives/${id}`, {});
  }

  get(id: string): Observable<Vacancy> {
    return this.httpClient.get<Vacancy>(`${this.basePath}/${id}`);
  }

  getByCompany(companyId: string): Observable<Vacancy[]> {
    return this.httpClient.get<Vacancy[]>(
      `${this.basePath}/companies/${companyId}`
    );
  }

  search(vacanciesFilter: VacanciesFilter): Observable<Vacancy[]> {
    return this.httpClient.post<Vacancy[]>(
      `${this.basePath}/search`,
      vacanciesFilter
    );
  }

  getRecomendationsForResume(
    resumeId: string,
    pageNumber: number,
    pageSize: number
  ): Observable<Vacancy[]> {
    return this.httpClient.get<Vacancy[]>(
      `${this.basePath}/recommendations/${resumeId}&pageNumber=${pageNumber}&pageSize=${pageSize}`
    );
  }

  getVacanciesByInteraction(
    userId: string,
    interactionType: InteractionType,
    pageNumber: number,
    pageSize: number
  ): Observable<Vacancy[]> {
    return this.httpClient.get<Vacancy[]>(
      `${this.basePath}/interactions/${userId}&type=${interactionType}&pageNumber=${pageNumber}&pageSize=${pageSize}`
    );
  }

  getDistinctRequirements(): Observable<string[]> {
    return this.httpClient.get<string[]>(`${this.basePath}/distinct/requirements`);
  }

  getDistinctSkills(): Observable<string[]> {
    return this.httpClient.get<string[]>(`${this.basePath}/distinct/skills`);
  }

  getDistinctTechnologies(): Observable<string[]> {
    return this.httpClient.get<string[]>(`${this.basePath}/distinct/technologies`);
  }
}
