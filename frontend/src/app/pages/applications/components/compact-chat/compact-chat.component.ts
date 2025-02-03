import { CommonModule } from '@angular/common';
import { Component, Input, SimpleChanges } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { UserRoles } from '../../../../core/enums/userRoles';
import Application from '../../../../core/models/application';
import Chat from '../../../../core/models/chat';
import Company from '../../../../core/models/company';
import User from '../../../../core/models/user';
import { CompaniesService } from '../../../../core/services/companies.service';
import { UsersService } from '../../../../core/services/users.service';

@Component({
  selector: 'app-compact-chat',
  standalone: true,
  imports: [CommonModule, MatButtonModule],
  templateUrl: './compact-chat.component.html',
})
export class CompactChatComponent {
  @Input() chat?: Chat;
  @Input() application?: Application;
  @Input() isActivated: boolean = false;
  @Input() forRole?: string;

  company?: Company;
  user?: User;

  constructor(
    private companiesService: CompaniesService,
    private usersService: UsersService
  ) {}

  ngOnInit() {
    this.loadData();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['application'] && changes['application'].currentValue) {
      this.loadData();
    }
  }

  private loadData() {
    switch (this.forRole) {
      case UserRoles.User:
        this.loadCompany();
        break;
      case UserRoles.Company:
        this.loadUser();
        break;
    }
  }

  private loadUser() {
    if (this.application?.userId) {
      this.usersService.get(this.application.userId).subscribe({
        next: (user) => {
          this.user = user;
        },
        error: (err) => console.error(err),
      });
    }
  }

  private loadCompany() {
    if (this.application?.vacancy?.companyId) {
      this.companiesService.get(this.application.vacancy.companyId).subscribe({
        next: (company) => (this.company = company),
        error: (err) => console.error(err),
      });
    }
  }

  get statusClass(): string {
    switch (this.chat?.application?.status.toLowerCase()) {
      case 'pending':
        return 'bg-yellow-400';
      case 'accepted':
        return 'bg-green-400';
      case 'rejected':
        return 'bg-red-400';
      default:
        return 'bg-gray-400';
    }
  }

  get activatedClass(): string {
    if (this.isActivated) {
      return 'bg-gray-400 hover:bg-gray-400';
    }

    return 'bg-gray-200 hover:bg-gray-300';
  }

  formatDate(lastMessageAt: Date | undefined): string {
    if (!lastMessageAt) {
      return '';
    }

    const date = new Date(lastMessageAt);
    const today = new Date();
    const yesterday = new Date();
    yesterday.setDate(today.getDate() - 1);

    if (date.toDateString() === today.toDateString()) {
      return date.toLocaleTimeString([], {
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit',
      });
    } else if (date.toDateString() === yesterday.toDateString()) {
      return date.toLocaleDateString('en', {
        day: 'numeric',
        month: 'long',
      });
    } else {
      return date.toLocaleDateString('en');
    }
  }

  isForUser() {
    return this.forRole === UserRoles.User;
  }

  isForCompany() {
    return this.forRole === UserRoles.Company;
  }
}
