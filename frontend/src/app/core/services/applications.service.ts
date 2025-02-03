import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiConfig } from '../../environments/api.config';
import Application from '../models/application';
import AddApplicationRequest from '../requests/applications/addApplicationRequest';
import UpdateApplicationRequest from '../requests/applications/updateApplicationRequest';

@Injectable({
  providedIn: 'root',
})
export class ApplicationsService {
  basePath = ApiConfig.applications;

  constructor(private httpClient: HttpClient) {}

  add(addRequest: AddApplicationRequest): Observable<string> {
    return this.httpClient.post<string>(`${this.basePath}`, addRequest);
  }

  update(updateRequest: UpdateApplicationRequest): Observable<string> {
    return this.httpClient.put<string>(`${this.basePath}`, updateRequest);
  }

  getPageByUser(
    userId: string,
    pageNumber: number,
    pageSize: number
  ): Observable<Application[]> {
    return this.httpClient.get<Application[]>(
      `${this.basePath}/users/${userId}&pageNumber=${pageNumber}&pageSize=${pageSize}`
    );
  }

  getPageByVacancy(
    vacancyId: string,
    pageNumber: number,
    pageSize: number
  ): Observable<Application[]> {
    return this.httpClient.get<Application[]>(
      `${this.basePath}/vacancies/${vacancyId}&pageNumber=${pageNumber}&pageSize=${pageSize}`
    );
  }

  getByIds(applicationsIds: string[]): Observable<Application[]> {
    return this.httpClient.post<Application[]>(`${this.basePath}/ids`, [
      ...applicationsIds,
    ]);
  }
}
