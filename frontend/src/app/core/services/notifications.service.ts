import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiConfig } from '../../environments/api.config';
import Notification from '../models/notification';

@Injectable({
  providedIn: 'root',
})
export class NotificationsService {
  private basePath: string = ApiConfig.notifications;

  constructor(private httpClient: HttpClient) {}

  viewMany(notificationsIds: string[]): Observable<void> {
    return this.httpClient.patch<void>(`${this.basePath}`, {
      notificationsIds,
    });
  }

  getUnreadedNotificationsByUser(
    recipientId: string
  ): Observable<Notification[]> {
    return this.httpClient.get<Notification[]>(
      `${this.basePath}/${recipientId}/unreaded`
    );
  }
}
