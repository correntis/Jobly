import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-message-context-menu',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './message-context-menu.component.html',
})
export class MessageContextMenuComponent {
  @Output() edit = new EventEmitter<void>();
  @Output() copy = new EventEmitter<void>();

  onEditClick() {
    this.edit.emit();
  }

  onCopyClick() {
    this.copy.emit();
  }
}
