import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { Observable } from 'rxjs';
import { ApplicationStatus } from '../../core/enums/applicationStatus';
import { UserRoles } from '../../core/enums/userRoles';
import { ChatsHub } from '../../core/hubs/chats.hub';
import { MessagesHub } from '../../core/hubs/messages.hub';
import Chat from '../../core/models/chat';
import Company from '../../core/models/company';
import Message from '../../core/models/message';
import { ApplicationsService } from '../../core/services/applications.service';
import { ChatsService } from '../../core/services/chatsService';
import { CompaniesService } from '../../core/services/companies.service';
import HashService from '../../core/services/hash.service';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { CompactChatComponent } from './components/compact-chat/compact-chat.component';
import { FullChatComponent } from './components/full-chat/full-chat.component';

@Component({
  selector: 'app-applications-chats-page',
  standalone: true,
  imports: [
    CommonModule,
    CompactChatComponent,
    FullChatComponent,
    HeaderComponent,
    MatIconModule,
  ],
  templateUrl: './applications-chats-page.component.html',
  styleUrl: './applications-chats-page.component.css',
})
export class ApplicationsChatsPageComponent {
  requestsId?: string;
  forRole?: string;

  company?: Company;

  chats: Chat[] = [];

  isLoadingChats: boolean = false;
  isFullLoaded: boolean = false;

  pageSize: number = 10;
  pageNumber: number = 1;

  activeChatIndex?: number;
  selectedChat?: Chat;

  // Filters
  selectedStatus: ApplicationStatus | 'All' = 'All';
  ApplicationStatus = ApplicationStatus;
  allChats: Chat[] = [];

  constructor(
    private activatedRoute: ActivatedRoute,
    private hashService: HashService,
    private applicationsService: ApplicationsService,
    private companiesService: CompaniesService,
    private chatsService: ChatsService,
    private cdRef: ChangeDetectorRef,
    private messagesHub: MessagesHub,
    private chatsHub: ChatsHub
  ) {
    this.loadRouteParams();
  }

  loadRouteParams() {
    this.activatedRoute.params.subscribe((params) => {
      this.requestsId = this.hashService.decrypt(params['userId']);
      this.forRole = this.hashService.decrypt(params['forRole']);

      console.log('open user id', this.requestsId);
      console.log('forRole', this.forRole);
    });
  }

  ngOnInit() {
    if (this.forRole === UserRoles.Company) {
      this.loadCompany();
    } else if (this.forRole === UserRoles.User) {
      this.loadData();
    }
  }

  loadData(): void {
    this.loadChats();
    this.loadConnections();
  }

  loadCompany(): void {
    if (this.requestsId) {
      this.companiesService.getByUser(this.requestsId).subscribe({
        next: (company) => {
          this.company = company;
          this.requestsId = company.id;
          this.loadData();
        },
        error: (err) => console.error(err),
      });
    }
  }

  loadChatsApplications(chats: Chat[]): void {
    const applicationIds = chats
      .map((chat) => chat.applicationId)
      .filter((id) => id !== undefined);

    if (applicationIds.length > 0) {
      this.applicationsService.getByIds(applicationIds).subscribe({
        next: (applications) => {
          chats.forEach((chat) => {
            const application = applications.find(
              (app) => app.id === chat.applicationId
            );

            chat.application = application;
          });
          this.cdRef.detectChanges();
        },
        error: (error) => {
          console.error(error);
        },
      });
    }
  }

  loadChats(): void {
    if (this.isLoadingChats || this.isFullLoaded) {
      return;
    }

    this.isLoadingChats = true;

    if (this.requestsId) {
      let observableChats: Observable<Chat[]> | undefined = undefined;

      if (this.forRole === UserRoles.Company && this.company?.id) {
        observableChats = this.chatsService.getPageByCompany(
          this.requestsId,
          this.pageNumber,
          this.pageSize
        );
      } else if (this.forRole === UserRoles.User) {
        observableChats = this.chatsService.getPageByUser(
          this.requestsId,
          this.pageNumber,
          this.pageSize
        );
      }

      if (observableChats) {
        observableChats.subscribe({
          next: (chats) => {
            if (chats.length === 0) {
              this.isFullLoaded = true;
              return;
            }
            this.loadChatsApplications(chats);

            // Update all chats for filtering
            if (this.allChats && this.allChats.length > 0) {
              this.allChats = [...this.allChats, ...chats];
            } else {
              this.allChats = [...chats];
            }

            // Apply filters to update displayed chats
            this.applyFilters();
            this.pageNumber++;
            this.isLoadingChats = false;
          },
          error: (err) => {
            console.error(err);
            this.isLoadingChats = false;
          },
        });
      }
    }
  }

  loadConnections() {
    if (this.requestsId) {
      this.messagesHub.buildConnection(this.requestsId);
      this.messagesHub.startConnection().subscribe({
        next: () => {
          this.messagesHub
            .receiveMessage()
            .subscribe((message) => this.handleReceivedMessage(message));

          this.messagesHub
            .receiveEditedMessage()
            .subscribe((message) => this.handleChangeMessage(message));

          this.messagesHub
            .receiveReadMessage()
            .subscribe((message) => this.handleChangeMessage(message));
        },
        error: (err) => console.error(err),
      });

      this.chatsHub.buildConnection(this.requestsId);
      this.chatsHub.startConnection().subscribe({
        next: () => {
          this.chatsHub.newChat().subscribe((chat) => this.handleNewChat(chat));
        },
        error: (err) => console.error(err),
      });
    }
  }

  handleReceivedMessage(message: Message) {
    if (this.allChats) {
      const chatIndex = this.allChats.findIndex(
        (chat) => chat.id === message.chatId
      );

      if (chatIndex !== -1) {
        const chat = this.allChats[chatIndex];

        if (chat.messages) {
          chat.messages = [message, ...chat.messages];
        }

        chat.lastMessageAt = message.sentAt;

        this.allChats.splice(chatIndex, 1);
        this.allChats.unshift(chat);

        this.applyFilters();
        this.correctSelectedIndex();

        this.cdRef.detectChanges();
      }
    }
  }

  handleChangeMessage(editedMessage: Message) {
    if (this.chats) {
      const chatIndex = this.chats.findIndex(
        (chat) => chat.id === editedMessage.chatId
      );

      if (chatIndex !== -1) {
        const chat = this.chats[chatIndex];

        if (chat.messages) {
          const messageIndex = chat.messages.findIndex(
            (message) => message.id === editedMessage.id
          );

          if (messageIndex !== -1) {
            const updatedMessages = [
              ...chat.messages.slice(0, messageIndex),
              editedMessage,
              ...chat.messages.slice(messageIndex + 1),
            ];

            chat.messages = updatedMessages;
            this.cdRef.detectChanges();
          }
        }
      }
    }
  }

  handleNewChat(chat: Chat) {
    this.allChats.unshift(chat);
    this.applyFilters();
    this.correctSelectedIndex();
    this.loadChatsApplications([chat]);
    this.cdRef.detectChanges();
  }

  openChat(chat: Chat | undefined): void {
    if (!chat) {
      return;
    }

    if (chat === this.selectedChat) {
      return;
    }

    if (this.chats) {
      const index = this.chats?.indexOf(chat);

      if (index !== -1) {
        chat.messages = [];
        this.isFullLoaded = false;
        this.activeChatIndex = index;
        this.selectedChat = chat;
      }
    }
  }

  activatedClass(index: number): string {
    if (index === this.activeChatIndex) {
      return 'bg-gray-600';
    }

    return '';
  }

  correctSelectedIndex(): void {
    this.activeChatIndex = this.chats?.findIndex(
      (chat) => chat.id === this.selectedChat?.id
    );
  }

  onChatsScroll(event: Event) {
    const element = event.target as HTMLElement;

    const elementHeight = element.scrollHeight - 1 - element.clientHeight;
    const currentPosition = Math.abs(element.scrollTop);

    const isOnDown = Math.abs(elementHeight - currentPosition) <= 30;

    if (isOnDown && !this.isLoadingChats) {
      this.loadChats();
    }
  }

  isForUser() {
    return this.forRole === UserRoles.User;
  }

  isForCompany() {
    return this.forRole === UserRoles.Company;
  }

  getSelectedChatRecipientId(): string | undefined {
    if (this.isForUser()) {
      return this.selectedChat?.companyId;
    } else if (this.isForCompany()) {
      return this.selectedChat?.userId;
    }

    return undefined;
  }

  onStatusFilterChange(status: ApplicationStatus | 'All'): void {
    this.selectedStatus = status;
    this.applyFilters();
  }

  applyFilters(): void {
    if (this.selectedStatus === 'All') {
      this.chats = [...this.allChats];
    } else {
      this.chats = this.allChats.filter(
        (chat) => chat.application?.status === this.selectedStatus
      );
    }
    this.correctSelectedIndex();
    this.cdRef.detectChanges();
  }

  getFilteredChatsCount(): number {
    return this.chats.length;
  }

  getStatusCount(status: ApplicationStatus | 'All'): number {
    if (status === 'All') {
      return this.allChats.length;
    }
    return this.allChats.filter(
      (chat) => chat.application?.status === status
    ).length;
  }

  onApplicationStatusChanged(chatId: string, newStatus: string): void {
    const chatIndex = this.allChats.findIndex((chat) => chat.id === chatId);
    if (chatIndex !== -1 && this.allChats[chatIndex].application) {
      this.allChats[chatIndex].application!.status = newStatus;
      this.applyFilters();
      this.cdRef.detectChanges();
    }
  }
}
