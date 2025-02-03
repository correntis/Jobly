import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Observable } from 'rxjs';
import { ApiConfig } from '../../environments/api.config';
import Message from '../models/message';
import SendMessageRequest from '../requests/messages/sendMessageRequest';

@Injectable({
  providedIn: 'root',
})
export class MessagesHub {
  private hubConnection?: signalR.HubConnection;

  constructor() {}

  buildConnection(userId: string): void {
    const uri = `${ApiConfig.messagesHub}?userId=${userId}`;

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
          console.error('error connecting to messages hub', error);
          observer.error(error);
        });
    });
  }

  async sendMessage(request: SendMessageRequest): Promise<void> {
    try {
      await this.hubConnection?.invoke('SendMessage', request);
    } catch (err) {
      console.error('error sending message ', err);
    }
  }

  receiveMessage(): Observable<Message> {
    return new Observable<Message>((observer) => {
      this.hubConnection?.on('ReceiveMessage', (message: Message) => {
        observer.next(message);
      });
    });
  }

  async editMessage(messageId: string, content: string): Promise<void> {
    try {
      await this.hubConnection?.invoke('EditMessage', {
        messageId,
        content,
      });
    } catch (err) {
      console.log('ERROR while sending message:', err);
    }
  }

  receiveEditedMessage(): Observable<Message> {
    return new Observable<Message>((observer) => {
      this.hubConnection?.on('EditMessage', (message: Message) => {
        observer.next(message);
      });
    });
  }

  async readMessage(messageId: string): Promise<void> {
    try {
      await this.hubConnection?.invoke('ReadMessage', {
        messageId,
      });
    } catch (err) {
      console.log('ERROR while sending message:', err);
    }
  }

  receiveReadMessage(): Observable<Message> {
    return new Observable<Message>((observer) => {
      this.hubConnection?.on('ReceiveReadMessage', (message: Message) => {
        observer.next(message);
      });
    });
  }
}
