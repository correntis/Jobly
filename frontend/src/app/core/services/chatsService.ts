import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiConfig } from '../../environments/api.config';
import Chat from '../models/chat';

@Injectable({
  providedIn: 'root',
})
export class ChatsService {
  private basePath: string = ApiConfig.chats;

  constructor(private httpClient: HttpClient) {}

  getByApplication(applicationId: string): Observable<Chat> {
    return this.httpClient.get<Chat>(
      `${this.basePath}/applications/${applicationId}`
    );
  }

  getPageByUser(
    userId: string,
    pageIndex: number,
    pageSize: number
  ): Observable<Chat[]> {
    return this.httpClient.get<Chat[]>(
      `${this.basePath}/users/${userId}&pageIndex=${pageIndex}&pageSize=${pageSize}`
    );
  }

  getPageByCompany(
    companyId: string,
    pageIndex: number,
    pageSize: number
  ): Observable<Chat[]> {
    return this.httpClient.get<Chat[]>(
      `${this.basePath}/companies/${companyId}&pageIndex=${pageIndex}&pageSize=${pageSize}`
    );
  }
}
