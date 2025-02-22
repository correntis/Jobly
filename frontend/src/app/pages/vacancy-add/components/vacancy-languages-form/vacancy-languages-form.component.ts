import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, Input } from '@angular/core';
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
  selector: 'app-vacancy-languages-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    CommonModule,
  ],
  templateUrl: './vacancy-languages-form.component.html',
})
export class VacancyLanguagesFormComponent {
  @Input() formArray!: FormArray;
  @Input() formArrayName!: string;

  languagesContainer: FormGroup;

  constructor(private fb: FormBuilder, private cdRef: ChangeDetectorRef) {
    this.languagesContainer = this.fb.group({});
  }

  ngOnInit(): void {
    this.languagesContainer.addControl(this.formArrayName, this.formArray);
  }

  lastLanguageIsEmpty(): boolean | undefined {
    var lastControl = this.formArray.controls[this.formArray.length - 1];

    if (lastControl) {
      const levelEmpty = lastControl.get('level')?.hasError('required');
      const nameEmpty = lastControl.get('name')?.hasError('required');

      return levelEmpty || nameEmpty;
    }

    return undefined;
  }

  addLanguage(): void {
    if (this.formArray.length > 0 && this.lastLanguageIsEmpty()) {
      this.formArray.markAllAsTouched();
      return;
    }

    const languageForm = this.fb.group({
      name: ['', Validators.required],
      level: ['', Validators.required],
    });

    this.formArray.push(languageForm);
    this.cdRef.detectChanges();
  }

  removeLanguage(index: number): void {
    this.formArray.removeAt(index);
    this.cdRef.detectChanges();
  }
}
