import { CommonModule } from '@angular/common';
import { Component, ElementRef, HostListener } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
import { NotificationStatus } from '../../../core/enums/notificationStatus';
import { NotificationType } from '../../../core/enums/notificationType';
import { NotificationsHub } from '../../../core/hubs/notifications.hub';
import Notification from '../../../core/models/notification';
import HashService from '../../../core/services/hash.service';
import { NotificationsService } from '../../../core/services/notifications.service';
import { EnvService } from '../../../environments/environment';

@Component({
  selector: 'app-notifications-menu',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  templateUrl: './notifications-menu.component.html',
})
export class NotificationsMenuComponent {
  notifications: Notification[] = [];
  pageIndex = 1;
  pageSize = 15;

  isMenuOpen: boolean = false;
  unreadCount: number = 0;

  NotificationStatus = NotificationStatus;
  NotificationType = NotificationType;

  constructor(
    private notificationsService: NotificationsService,
    private notificationHub: NotificationsHub,
    private envService: EnvService,
    private router: Router,
    private hashService: HashService,
    private elementRef: ElementRef
  ) {}

  ngOnInit() {
    this.loadNotifications();
    this.loadConnections();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.isMenuOpen = false;
    }
  }

  loadNotifications(): void {
    this.notificationsService
      .getUnreadedNotificationsByUser(this.envService.getUserId())
      .subscribe({
        next: (notifications) => {
          this.notifications = notifications;
          this.updateUnreadCount();
        },
        error: (err) => console.error(err),
      });
  }

  loadConnections(): void {
    this.notificationHub.buildConnection(this.envService.getUserId());
    this.notificationHub.startConnection().subscribe({
      next: () => {
        this.notificationHub
          .receiveNotification()
          .subscribe((notification) =>
            this.handleReceiveNotification(notification)
          );
      },
      error: (err) => console.error(err),
    });
  }

  handleReceiveNotification(notification: Notification) {
    this.notifications.push(notification);
    this.updateUnreadCount();
  }

  toggleMenu(): void {
    this.isMenuOpen = !this.isMenuOpen;
  }

  viewNotifications(ids: string[]) {
    this.notificationsService.viewMany(ids).subscribe({
      next: () => {
        this.updateViewStatus(ids);
        this.updateUnreadCount();
      },
      error: (err) => console.error(err),
    });
  }

  markAsViewed(notification: Notification): void {
    if (notification.status === NotificationStatus.Sent) {
      this.viewNotifications([notification.id]);
    }
  }

  markAllAsViewed() {
    this.viewNotifications(
      this.notifications
        .filter((notif) => notif.status === NotificationStatus.Sent)
        .map((notif) => notif.id)
    );
    this.isMenuOpen = false;
  }

  updateViewStatus(notificationsIds: string[]) {
    this.notifications.forEach((notif) => {
      if (notificationsIds.includes(notif.id)) {
        notif.status = NotificationStatus.Viewed;
      }
    });
  }

  updateUnreadCount(): void {
    this.unreadCount = this.notifications.filter(
      (notification) => notification.status === NotificationStatus.Sent
    ).length;
  }

  goToCompany(notification: Notification) {
    const companyId = notification.metadata['CompanyId'];

    if (companyId) {
      const hashedId = this.hashService.encrypt(companyId);

      const url = this.router.serializeUrl(
        this.router.createUrlTree(['/company', hashedId])
      );

      window.open(url, '_blank');
    }
  }

  goToVacancy(notification: Notification) {
    const vacancyId = notification.metadata['VacancyId'];

    if (vacancyId) {
      const hashedId = this.hashService.encrypt(vacancyId);

      const url = this.router.serializeUrl(
        this.router.createUrlTree(['/vacancy', hashedId])
      );

      window.open(url, '_blank');
    }
  }
}
