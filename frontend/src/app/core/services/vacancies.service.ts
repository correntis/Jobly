import { VacanciesFilter } from './../models/vacancies/vacanciesFilter';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EnvParams } from '../../environments/environment';
import { ApiConfig } from '../../environments/api.config';
import { AddVacancyRequest } from '../requests/vacancies/addVacancyRequest';
import { Observable } from 'rxjs';
import Vacancy from '../models/vacancies/vacancy';

@Injectable({
  providedIn: 'root',
})
export class VacanciesService {
  basePath = ApiConfig.vacancies;

  constructor(private httpClient: HttpClient) {}

  add(addVacancyRequest: AddVacancyRequest): Observable<string> {
    return this.httpClient.post<string>(`${this.basePath}`, addVacancyRequest);
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
      `${this.basePath}/recommendations//${resumeId}&pageNumber=${pageNumber}&pageSize=${pageSize}`
    );
  }
}
