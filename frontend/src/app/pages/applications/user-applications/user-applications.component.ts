import { ChangeDetectorRef, Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import HashService from '../../../core/services/hash.service';
import { ApplicationsService } from '../../../core/services/applications.service';
import { CompactUserChatComponent } from '../../../features/applications/compact-user-chat/compact-user-chat.component';
import { CommonModule } from '@angular/common';
import { ChatsService } from '../../../core/services/chatsService';
import Chat from '../../../core/models/chat';
import { FullChatComponent } from '../../../features/applications/full-chat/full-chat.component';
import { MessagesHub } from '../../../core/hubs/messages.hub';
import Message from '../../../core/models/message';
import { ChatsHub } from '../../../core/hubs/chats.hub';
import Company from '../../../core/models/company';
import { UserRoles } from '../../../core/enums/userRoles';
import { CompaniesService } from '../../../core/services/companies.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-user-applications',
  standalone: true,
  imports: [CommonModule, CompactUserChatComponent, FullChatComponent],
  templateUrl: './user-applications.component.html',
  styleUrl: './user-applications.component.css',
})
export class UserApplicationsComponent {
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
    this.activatedRoute.params.subscribe((params) => {
      this.requestsId = this.hashService.decrypt(params['userId']);
      this.forRole = this.hashService.decrypt(params['forRole']);
    });
  }

  ngOnInit() {
    if (this.requestsId) {
      if (this.forRole === UserRoles.Company) {
        this.companiesService.getByUser(this.requestsId).subscribe({
          next: (company) => {
            this.company = company;
            this.requestsId = company.id;
            this.loadData();
          },
          error: (err) => console.error(err),
        });
      } else {
        this.loadData();
      }
    }
  }

  loadData() {
    this.loadChats();
    this.loadConnections();
  }

  loadChatsApplications(chats: Chat[]) {
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
          console.error('Ошибка при получении приложений:', error);
        },
      });
    }
  }

  loadChats() {
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

            if (this.chats) {
              this.chats = [...this.chats, ...chats];
            } else {
              this.chats = chats;
            }

            this.pageNumber++;
            this.isLoadingChats = false;
          },
          error: (err) => {
            console.error('error loading chats ', err);
            this.isLoadingChats = false;
          },
        });
      }
    }
  }

  openChat(chat: Chat | undefined) {
    if (!chat) {
      return;
    }

    if (chat === this.selectedChat) {
      return;
    }

    if (this.chats) {
      const index = this.chats?.indexOf(chat);

      if (index !== -1) {
        this.isFullLoaded = false;
        this.activeChatIndex = index;
        chat.messages = [];
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

  correctSelectedIndex() {
    this.activeChatIndex = this.chats?.findIndex(
      (chat) => chat.id === this.selectedChat?.id
    );
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
    if (this.chats) {
      const chatIndex = this.chats.findIndex(
        (chat) => chat.id === message.chatId
      );

      if (chatIndex !== -1) {
        const chat = this.chats[chatIndex];

        if (chat.messages) {
          chat.messages = [message, ...chat.messages];
        }

        chat.lastMessageAt = message.sentAt;

        this.chats.splice(chatIndex, 1);
        this.chats.unshift(chat);

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
    this.chats.unshift(chat);

    this.correctSelectedIndex();
    this.loadChatsApplications([chat]);
    this.cdRef.detectChanges();
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
}
