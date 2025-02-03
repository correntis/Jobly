import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-dynamic-form-array',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CommonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatButtonModule,
  ],
  templateUrl: './dynamic-form-array.component.html',
})
export class DynamicFormArrayComponent implements OnInit {
  @Input() formArrayName!: string;
  @Input() label!: string;
  @Input() placeholder!: string;
  @Input() formArray!: FormArray;

  container: FormGroup;

  constructor(private fb: FormBuilder) {
    this.container = this.fb.group({});
  }

  ngOnInit(): void {
    this.container.addControl(this.formArrayName, this.formArray);
  }

  addItem() {
    if (this.formArray.length > 0 && this.lastItemIsEmpty()) {
      this.formArray.markAllAsTouched();
      return;
    }
    this.formArray.push(this.fb.group({ name: ['', Validators.required] }));
  }

  remove(index: number) {
    this.formArray.removeAt(index);
  }

  lastItemIsEmpty(): boolean | undefined {
    return this.formArray.controls[this.formArray.length - 1]
      ?.get('name')
      ?.hasError('required');
  }
}
