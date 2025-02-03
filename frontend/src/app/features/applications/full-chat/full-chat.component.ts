import { VacanciesFilter } from './../../../core/models/vacancies/vacanciesFilter';
import { bootstrapApplication } from '@angular/platform-browser';
import { MessagesHub } from './../../../core/hubs/messages.hub';
import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  HostListener,
  Input,
  NgModule,
  SimpleChanges,
  ViewChild,
} from '@angular/core';
import Chat from '../../../core/models/chat';
import Message from '../../../core/models/message';
import MessagesService from '../../../core/services/messages.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MessageType } from '../../../core/enums/messageType';
import SendMessageRequest from '../../../core/requests/messages/sendMessageRequest';
import { EnvService } from '../../../environments/environment';
import { UserMessageComponent } from '../user-message/user-message.component';
import { ContextMenuMessageComponent } from '../context-menu-message/context-menu-message.component';
import { SimpleInputFormComponent } from '../../../shared/components/simple-input-form/simple-input-form.component';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterModule } from '@angular/router';
import HashService from '../../../core/services/hash.service';
import { UserRoles } from '../../../core/enums/userRoles';
import User from '../../../core/models/user';
import { UsersService } from '../../../core/services/users.service';

@Component({
  selector: 'app-full-chat',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    UserMessageComponent,
    ContextMenuMessageComponent,
    SimpleInputFormComponent,
    MatIconModule,
    RouterModule,
  ],
  templateUrl: './full-chat.component.html',
})
export class FullChatComponent {
  @ViewChild('scrollContainer') scrollContainer!: ElementRef;

  @Input() chat?: Chat;
  @Input() messages: Message[] = [];
  @Input() currentSenderId?: string;
  @Input() currentRecipientId?: string;
  @Input() forRole?: string;

  user?: User;

  isLoading: boolean = false;
  isFullLoaded: boolean = false;

  pageIndex: number = 1;
  pageSize: number = 15;

  showContextMenu: boolean = false;
  contextMenuX: number = 0;
  contextMenuY: number = 0;
  isEditing: boolean = false;
  selectedMessage?: Message;
  selectedMessageIndex: number = -1;

  private observer!: IntersectionObserver;

  constructor(
    private messagesService: MessagesService,
    private usersService: UsersService,
    private messagesHub: MessagesHub,
    private envService: EnvService,
    private cdRef: ChangeDetectorRef,
    private hashService: HashService,
    private router: Router
  ) {}

  ngOnInit() {
    if (this.isForCompany()) {
      this.loadUser();
    }

    this.loadMessages();
  }

  ngAfterViewInit() {
    this.observer = new IntersectionObserver(
      (entries) => this.handleIntersection(entries),
      {
        root: this.scrollContainer.nativeElement,
        threshold: 0.5,
      }
    );
  }

  observeMessages() {
    setTimeout(() => {
      this.messages.forEach((message, index) => {
        const messageElement = this.scrollContainer.nativeElement.querySelector(
          `#message-${index}`
        );

        if (messageElement && this.isUnreadedRecipientMessage(message)) {
          this.observer.observe(messageElement);
        }
      });
    }, 200);
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['chat']?.previousValue && changes['chat']?.currentValue) {
      this.pageIndex = 1;
      this.pageSize = 15;
      this.messages = [];
      this.isFullLoaded = false;
      this.loadMessages();
    }

    if (changes['messages']?.currentValue) {
      this.messages = changes['messages'].currentValue;
      this.observeMessages();
    }
  }

  handleIntersection(entries: IntersectionObserverEntry[]) {
    entries.forEach((entry) => {
      if (entry.isIntersecting) {
        const messageIndex = parseInt(
          entry.target.getAttribute('id')?.replace('message-', '') || '-1',
          10
        );

        if (messageIndex >= 0 && this.messages[messageIndex]) {
          const message = this.messages[messageIndex];

          if (this.isUnreadedRecipientMessage(message)) {
            this.readMessage(message.id);
          }
        }
      }
    });
  }

  loadUser() {
    if (this.currentRecipientId) {
      this.usersService.get(this.currentRecipientId).subscribe({
        next: (user) => (this.user = user),
        error: (err) => console.error(err),
      });
    }
  }

  loadMessages() {
    if (this.isLoading || this.isFullLoaded) {
      return;
    }

    this.isLoading = true;

    if (this.chat) {
      this.messagesService
        .getPageByChat(this.chat?.id, this.pageIndex, this.pageSize)
        .subscribe({
          next: (newMessages) => {
            if (newMessages.length > 0) {
              this.messages.push(...newMessages);
              this.pageIndex++;
            } else {
              this.isFullLoaded = true;
            }
            this.isLoading = false;

            this.observeMessages();
          },
          error: (err) => {
            console.error(err);
            this.isLoading = false;
          },
        });
    }
  }

  getRecipientId(): string | undefined {
    if (this.envService.isUser()) {
      return this.chat?.companyId;
    } else if (this.envService.isCompany()) {
      return this.chat?.userId;
    }
    return undefined;
  }

  sendMessage(newContent: string) {
    if (newContent.trim() && this.chat) {
      if (this.currentRecipientId && this.currentSenderId) {
        const sendMessageRequest: SendMessageRequest = {
          chatId: this.chat.id,
          recipientId: this.currentRecipientId,
          senderId: this.currentSenderId,
          content: newContent,
        };

        this.messagesHub.sendMessage(sendMessageRequest);
      }
    }
  }

  readMessage(messageId: string) {
    this.messagesHub.readMessage(messageId);
  }

  onScroll(event: Event) {
    const element = event.target as HTMLElement;

    const elementHeight = element.scrollHeight - 1 - element.clientHeight;
    const currentPosition = Math.abs(element.scrollTop);

    const isOnTop = Math.abs(elementHeight - currentPosition) <= 30;

    if (isOnTop && !this.isLoading) {
      this.loadMessages();
    }
  }

  @HostListener('document:click')
  onDocumentClick() {
    if (!this.isEditing) {
      this.clearContext();
    }
  }

  onRightClick(event: MouseEvent, message: any, index: number) {
    event.preventDefault();

    if (this.selectedMessage) {
      return;
    }

    this.contextMenuX = event.clientX;
    this.contextMenuY = event.clientY;

    this.selectedMessage = message;
    this.selectedMessageIndex = index;
    this.showContextMenu = true;
  }

  onEditMessage() {
    if (this.selectedMessage) {
      this.showContextMenu = false;
      this.isEditing = true;

      this.cdRef.detectChanges();
    }
  }

  onCopyClick() {
    if (this.selectedMessage) {
      navigator.clipboard.writeText(this.selectedMessage.content);
    }
  }

  cancelEditing() {
    this.clearContext();
    this.isEditing = false;

    this.cdRef.detectChanges();
  }

  clearContext() {
    this.selectedMessage = undefined;
    this.selectedMessageIndex = -1;
    this.showContextMenu = false;
  }

  submitEditMessage(newContent: string) {
    if (this.selectedMessage) {
      if (newContent !== this.selectedMessage.content) {
        this.messagesHub.editMessage(this.selectedMessage.id, newContent);
      }
    }

    this.isEditing = false;
    this.cdRef.detectChanges();
  }

  goToCompany() {
    const companyId = this.chat?.application?.vacancy?.companyId;

    if (companyId) {
      const hashedId = this.hashService.encrypt(companyId);

      this.router.navigate(['company', hashedId]);
    }
  }

  isUnreadedRecipientMessage(message: Message): boolean {
    const isRecipient = this.currentSenderId !== message.senderId;
    const isUnreaded = message.isRead !== true;

    return isRecipient && isUnreaded;
  }

  goToResume() {
    // TODO redirect to user resume page
  }

  isForUser() {
    return this.forRole === UserRoles.User;
  }

  isForCompany() {
    return this.forRole === UserRoles.Company;
  }
}
