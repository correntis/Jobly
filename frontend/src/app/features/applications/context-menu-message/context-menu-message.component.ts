import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-context-menu-message',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './context-menu-message.component.html',
})
export class ContextMenuMessageComponent {
  @Output() edit = new EventEmitter<void>();
  @Output() copy = new EventEmitter<void>();

  onEditClick() {
    this.edit.emit();
  }

  onCopyClick() {
    this.copy.emit();
  }
}
