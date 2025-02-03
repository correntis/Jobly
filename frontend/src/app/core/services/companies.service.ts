import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import Company from '../models/company';
import { HttpClient } from '@angular/common/http';
import AddCompanyRequest from '../requests/companies/addCompanyRequest';
import UpdateCompanyRequest from '../requests/companies/updateCompanyRequest';
import { ApiConfig } from '../../environments/api.config';

@Injectable({
  providedIn: 'root',
})
export class CompaniesService {
  basePath = ApiConfig.companies;
  constructor(private httpClient: HttpClient) {}

  get(id: string): Observable<Company> {
    return this.httpClient.get<Company>(`${this.basePath}/${id}`);
  }

  getByUser(userId: string): Observable<Company> {
    return this.httpClient.get<Company>(`${this.basePath}/users/${userId}`);
  }

  add(addRequest: AddCompanyRequest, image: File | null): Observable<string> {
    const formData = new FormData();

    Object.entries(addRequest).forEach(([key, value]) => {
      if (value !== null && value !== undefined) {
        formData.append(key, value);
      }
    });

    if (image) {
      formData.append('image', image, image.name);
    }

    return this.httpClient.post<string>(`${this.basePath}`, formData);
  }

  update(
    updateRequest: UpdateCompanyRequest,
    image: File | null
  ): Observable<string> {
    const formData = new FormData();

    Object.entries(updateRequest).forEach(([key, value]) => {
      if (value !== null && value !== undefined) {
        formData.append(key, value);
      }
    });

    if (image) {
      formData.append('image', image, image.name);
    }

    return this.httpClient.put<string>(`${this.basePath}`, formData);
  }

  viewResume(companyId: string, resumeId: string): Observable<string> {
    return this.httpClient.post<string>(`${this.basePath}/views`, {
      companyId,
      resumeId,
    });
  }

  delete(id: string): Observable<string> {
    return this.httpClient.delete<string>(`${this.basePath}/${id}}`);
  }
}
