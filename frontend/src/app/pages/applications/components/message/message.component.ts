import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MessageType } from '../../../../core/enums/messageType';
import Message from '../../../../core/models/message';

@Component({
  selector: 'app-message',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  templateUrl: './message.component.html',
})
export class MessageComponent {
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
