import { CommonModule } from '@angular/common';
import {
  AfterViewInit,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-simple-input-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './simple-input-form.component.html',
})
export class SimpleInputFormComponent implements OnInit, AfterViewInit {
  @ViewChild('formInput') formInputElement!: ElementRef;

  @Output() submit = new EventEmitter<string>();
  @Input() placeHolder?: string;
  @Input() buttonText?: string;
  @Input() input?: string;
  @Input() isFocusInput: boolean = false;

  newInput: string = '';

  ngOnInit() {
    if (this.input) {
      this.newInput = this.input;
    }
  }

  ngAfterViewInit() {
    this.formInputElement.nativeElement.focus();
  }

  onSubmit() {
    if (this.newInput.trim()) {
      this.submit.emit(this.newInput);
      this.newInput = '';
    }
  }
}
