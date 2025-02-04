import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';

@Component({
  selector: 'app-vacancy-experience-form',
  standalone: true,
  imports: [
    MatIconModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    CommonModule,
  ],
  templateUrl: './vacancy-experience-form.component.html',
})
export class VacancyExperienceFormComponent {
  @Input() jobExperiencesForm!: FormGroup;

  isVisible: boolean = false;

  buttonIcon: 'add' | 'delete' = 'add';
  buttonColor: 'accent' | 'warn' = 'accent';

  constructor() {}

  changeVisibility() {
    this.isVisible = !this.isVisible;

    if (this.isVisible) {
      this.buttonIcon = 'delete';
      this.buttonColor = 'warn';
    } else {
      this.buttonIcon = 'add';
      this.buttonColor = 'accent';
    }
  }
}
