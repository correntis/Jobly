import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Observable } from 'rxjs';
import { ApiConfig } from '../../environments/api.config';
import Notification from '../models/notification';

@Injectable({ providedIn: 'root' })
export class NotificationsHub {
  private hubConnection?: signalR.HubConnection;

  constructor() {}

  buildConnection(userId: string): void {
    const uri = `${ApiConfig.notificationsHub}?userId=${userId}`;

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
          console.error(error);
          observer.error(error);
        });
    });
  }

  receiveNotification(): Observable<Notification> {
    return new Observable<Notification>((observer) => {
      this.hubConnection?.on(
        'ReceiveNotification',
        (notification: Notification) => {
          observer.next(notification);
        }
      );
    });
  }
}
