import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiConfig } from '../../environments/api.config';
import Certification from '../models/resumes/certification';
import Education from '../models/resumes/education';
import JobExperience from '../models/resumes/jobExperience';
import Language from '../models/resumes/language';
import Project from '../models/resumes/project';
import Resume from '../models/resumes/resume';
import AddResumeRequest from '../requests/resumes/addResumeRequest';
import UpdateResumeRequest from '../requests/resumes/updateResumeRequest';

@Injectable({
  providedIn: 'root',
})
export class ResumesService {
  private basePath: string = ApiConfig.resumes;

  constructor(private httpClient: HttpClient) {}

  add(addRequest: AddResumeRequest): Observable<Resume> {
    return this.httpClient.post<Resume>(`${this.basePath}`, addRequest);
  }

  update(updateRequest: UpdateResumeRequest): Observable<Resume> {
    return this.httpClient.put<Resume>(`${this.basePath}`, updateRequest);
  }

  updateCertifications(
    id: string,
    certifications: Certification[]
  ): Observable<string> {
    return this.httpClient.put<string>(`${this.basePath}/certifications`, {
      id,
      certifications,
    });
  }

  updateEducations(id: string, educations: Education[]): Observable<string> {
    return this.httpClient.put<string>(`${this.basePath}/educations`, {
      id,
      educations,
    });
  }

  updateJobExpiriences(
    id: string,
    jobExperiences: JobExperience[]
  ): Observable<string> {
    return this.httpClient.put<string>(`${this.basePath}/experiences`, {
      id,
      jobExperiences,
    });
  }

  updateLanguages(id: string, languages: Language[]): Observable<string> {
    return this.httpClient.put<string>(`${this.basePath}/languages`, {
      id,
      languages,
    });
  }

  updateProjects(id: string, projects: Project[]): Observable<string> {
    return this.httpClient.put<string>(`${this.basePath}/projects`, {
      id,
      projects,
    });
  }

  get(id: string): Observable<Resume> {
    return this.httpClient.get<Resume>(`${this.basePath}/${id}`);
  }

  getByUser(userId: string): Observable<Resume> {
    return this.httpClient.get<Resume>(`${this.basePath}/users/${userId}`);
  }

  delete(id: string): Observable<string> {
    return this.httpClient.delete<string>(`${this.basePath}/${id}`);
  }
}
