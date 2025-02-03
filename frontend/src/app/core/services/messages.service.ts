import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiConfig } from '../../environments/api.config';
import Message from '../models/message';

@Injectable({
  providedIn: 'root',
})
export default class MessagesService {
  basePath: string = ApiConfig.messages;

  constructor(private httpClient: HttpClient) {}

  getPageByChat(
    chatId: string,
    pageIndex: number,
    pageSize: number
  ): Observable<Message[]> {
    return this.httpClient.get<Message[]>(
      `${this.basePath}/${chatId}&pageIndex=${pageIndex}&pageSize=${pageSize}`
    );
  }

  searchInChat(chatId: string, content: string): Observable<Message[]> {
    return this.httpClient.get<Message[]>(
      `${this.basePath}/${chatId}&content=${content}`
    );
  }
}
