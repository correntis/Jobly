import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiConfig } from '../../environments/api.config';
import Application from '../models/application';
import ApplicationsStatusCounts from '../models/applicationsStatusCounts';
import AddApplicationRequest from '../requests/applications/addApplicationRequest';
import UpdateApplicationRequest from '../requests/applications/updateApplicationRequest';

@Injectable({
  providedIn: 'root',
})
export class ApplicationsService {
  private basePath: string = ApiConfig.applications;

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

  getByUserAndVacancy(userId: string, vacancyId: string): Observable<Application | null> {
    return this.httpClient.get<Application | null>(
      `${this.basePath}/users/${userId}/vacancies/${vacancyId}`
    );
  }

  getStatusCountsByUser(userId: string): Observable<ApplicationsStatusCounts> {
    return this.httpClient.get<ApplicationsStatusCounts>(
      `${this.basePath}/users/${userId}/status-counts`
    );
  }

  getStatusCountsByCompany(companyId: string): Observable<ApplicationsStatusCounts> {
    return this.httpClient.get<ApplicationsStatusCounts>(
      `${this.basePath}/companies/${companyId}/status-counts`
    );
  }
}
