import { HashedCookieService } from './../services/hashedCookie.service';
import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { EnvParams } from '../../environments/environment';
import { ApiConfig } from '../../environments/api.config';
import { Observable } from 'rxjs';
import Chat from '../models/chat';

@Injectable({
  providedIn: 'root',
})
export class ChatsHub {
  private hubConnection?: signalR.HubConnection;

  constructor() {}

  buildConnection(userId: string) {
    const uri = `${ApiConfig.chatsHub}?userId=${userId}`;

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(uri, {
        transport:
          signalR.HttpTransportType.WebSockets |
          signalR.HttpTransportType.ServerSentEvents |
          signalR.HttpTransportType.LongPolling,
      })
      .build();
  }

  startConnection(): Observable<void> {
    return new Observable<void>((observer) => {
      this.hubConnection
        ?.start()
        .then(() => {
          observer.next();
          observer.complete();
        })
        .catch((error) => {
          console.error('error connecting to chats hub', error);
          observer.error(error);
        });
    });
  }

  newChat(): Observable<Chat> {
    return new Observable<Chat>((observer) => {
      this.hubConnection?.on('NewChat', (chat: Chat) => {
        observer.next(chat);
      });
    });
  }
}
