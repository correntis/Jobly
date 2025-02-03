import { Component, Input } from '@angular/core';
import Message from '../../../core/models/message';
import { CommonModule } from '@angular/common';
import { MessageType } from '../../../core/enums/messageType';

@Component({
  selector: 'app-user-message',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './user-message.component.html',
})
export class UserMessageComponent {
  @Input() message?: Message;
  @Input() senderId?: string;

  isCreationMessage(): boolean {
    return this.message?.type === MessageType.Creation;
  }

  isUserMessage(): boolean {
    return this.message?.type === MessageType.User;
  }

  isSender() {
    return this.message?.senderId === this.senderId;
  }
}
