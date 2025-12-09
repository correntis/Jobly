import { CommonModule } from '@angular/common';
import { Component, ElementRef, HostListener, ViewChild, AfterViewInit } from '@angular/core';
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
export class NotificationsMenuComponent implements AfterViewInit {
  notifications: Notification[] = [];
  pageIndex = 1;
  pageSize = 15;

  isMenuOpen: boolean = false;
  unreadCount: number = 0;
  menuLeft: string = '0px';

  NotificationStatus = NotificationStatus;
  NotificationType = NotificationType;

  @ViewChild('buttonRef', { static: false }) buttonRef!: ElementRef;

  constructor(
    private notificationsService: NotificationsService,
    private notificationHub: NotificationsHub,
    private envService: EnvService,
    private router: Router,
    private hashService: HashService,
    private elementRef: ElementRef
  ) {}

  ngAfterViewInit() {
    this.updateMenuPosition();
  }

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

  @HostListener('window:resize', ['$event'])
  onResize(): void {
    if (this.isMenuOpen) {
      this.updateMenuPosition();
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
    if (this.isMenuOpen) {
      setTimeout(() => this.updateMenuPosition(), 0);
    }
  }

  updateMenuPosition(): void {
    if (this.buttonRef?.nativeElement) {
      const buttonRect = this.buttonRef.nativeElement.getBoundingClientRect();
      const menuWidth = 384; // w-96 = 384px
      const leftPosition = buttonRect.left;
      const rightSpace = window.innerWidth - buttonRect.left;
      
      // Если меню не помещается справа, сдвигаем влево
      if (rightSpace < menuWidth) {
        this.menuLeft = `${leftPosition - (menuWidth - rightSpace)}px`;
      } else {
        this.menuLeft = `${leftPosition}px`;
      }
    }
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
