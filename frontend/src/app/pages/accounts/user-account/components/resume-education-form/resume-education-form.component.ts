import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  Input,
  SimpleChanges,
} from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import Education from '../../../../../core/models/resumes/education';
import { ResumesService } from '../../../../../core/services/resumes.service';

@Component({
  selector: 'app-resume-education-form',
  standalone: true,
  imports: [
    MatIconModule,
    ReactiveFormsModule,
    CommonModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    FormsModule,
    MatDatepickerModule,
    MatNativeDateModule,
  ],
  templateUrl: './resume-education-form.component.html',
})
export class ResumeEducationFormComponent {
  @Input() resumeEducations: Education[] | undefined;
  @Input() resumeId: string | undefined;

  educationForm: FormGroup;

  constructor(
    private resumesService: ResumesService,
    private fb: FormBuilder,
    private cdRef: ChangeDetectorRef
  ) {
    this.educationForm = this.fb.group({
      educations: this.fb.array([]),
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (!this.resumeEducations) {
      return;
    }

    if (changes['resumeEducations'] && this.resumeEducations.length) {
      this.educations.clear();
      this.patchEducations();
    }
  }

  get educations(): FormArray {
    return this.educationForm.get('educations') as FormArray;
  }

  lastEducationIsEmpty(): boolean | undefined {
    var lastControl = this.educations.controls[this.educations.length - 1];

    if (lastControl) {
      const institutionEmpty = lastControl
        .get('institution')
        ?.hasError('required');
      const degreeEmpty = lastControl.get('degree')?.hasError('required');
      const startDateEmpty = lastControl.get('startDate')?.hasError('required');
      const endDateEmpty = lastControl.get('endDate')?.hasError('required');

      return institutionEmpty || degreeEmpty || startDateEmpty || endDateEmpty;
    }

    return undefined;
  }

  addEducation(): void {
    if (this.educations.length > 0 && this.lastEducationIsEmpty()) {
      this.educations.markAllAsTouched();
      return;
    }

    const educationForm = this.fb.group({
      institution: ['', Validators.required],
      degree: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
    });
    this.educations.push(educationForm);
    this.cdRef.detectChanges();
  }

  removeEducation(index: number): void {
    this.educations.removeAt(index);
    this.cdRef.detectChanges();
  }

  patchEducations(): void {
    if (!this.resumeEducations) {
      return;
    }

    this.resumeEducations.forEach((education) => {
      this.educations.push(
        this.fb.group({
          institution: [education.institution, Validators.required],
          degree: [education.degree, Validators.required],
          startDate: [education.startDate, Validators.required],
          endDate: [education.endDate, Validators.required],
        })
      );
    });
  }

  saveEducations() {
    if (!this.educationForm.valid) {
      alert('pls fill out educations correctly');
      return;
    }

    const { educations }: { educations: Education[] } =
      this.educationForm.value;

    if (this.resumeEducations && this.resumeId) {
      this.resumesService
        .updateEducations(this.resumeId, educations)
        .subscribe({
          error: (err) => console.error(err),
        });
    }
  }
}
