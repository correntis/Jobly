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
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import JobExperience from '../../../../../core/models/resumes/jobExperience';
import { ResumesService } from '../../../../../core/services/resumes.service';

@Component({
  selector: 'app-resume-experiences-form',
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
  templateUrl: './resume-experiences-form.component.html',
})
export class ResumeExperiencesFormComponent {
  @Input() resumeJobExperiences: JobExperience[] | undefined;
  @Input() resumeId: string | undefined;

  jobExperiencesForm: FormGroup;

  constructor(
    private resumesService: ResumesService,
    private fb: FormBuilder,
    private cdRef: ChangeDetectorRef
  ) {
    this.jobExperiencesForm = this.fb.group({
      jobExperiences: this.fb.array([]),
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (!this.resumeJobExperiences) {
      return;
    }

    if (changes['resumeJobExperiences'] && this.resumeJobExperiences.length) {
      this.jobExperiences.clear();
      this.patchJobExperiences();
    }
  }

  get jobExperiences(): FormArray {
    return this.jobExperiencesForm.get('jobExperiences') as FormArray;
  }

  getResponsibilities(jobExperienceIndex: number): FormArray {
    return this.jobExperiences
      .at(jobExperienceIndex)
      .get('responsibilities') as FormArray;
  }

  lastResponsibilityIsEmpty(index: number): boolean | undefined {
    const responsibilitiesArray = this.getResponsibilities(index);
    return responsibilitiesArray.controls.some((control) =>
      control.hasError('required')
    );
  }

  lastJobExperienceIsEmpty(): boolean | undefined {
    const lastControl =
      this.jobExperiences.controls[this.jobExperiences.length - 1];

    if (lastControl) {
      const jobTitleEmpty = lastControl.get('jobTitle')?.hasError('required');
      const companyNameEmpty = lastControl
        .get('companyName')
        ?.hasError('required');
      const startDateEmpty = lastControl.get('startDate')?.hasError('required');
      const endDateEmpty = lastControl.get('endDate')?.hasError('required');

      return (
        jobTitleEmpty ||
        companyNameEmpty ||
        startDateEmpty ||
        endDateEmpty ||
        this.lastResponsibilityIsEmpty(this.jobExperiences.length - 1)
      );
    }

    return undefined;
  }

  addJobExperience(): void {
    if (this.jobExperiences.length > 0 && this.lastJobExperienceIsEmpty()) {
      this.jobExperiences.markAllAsTouched();
      return;
    }

    const jobExperienceForm = this.fb.group({
      jobTitle: ['', Validators.required],
      companyName: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      responsibilities: this.fb.array([]),
    });
    this.jobExperiences.push(jobExperienceForm);
    this.cdRef.detectChanges();
  }

  addResponsibility(jobExperienceIndex: number): void {
    if (this.lastResponsibilityIsEmpty(jobExperienceIndex)) {
      this.jobExperiences.markAllAsTouched();
      return;
    }

    const respArray = this.getResponsibilities(jobExperienceIndex);
    respArray.push(this.fb.control('', Validators.required));

    this.cdRef.detectChanges();
  }

  removeJobExperience(index: number): void {
    this.jobExperiences.removeAt(index);
    this.cdRef.detectChanges();
  }

  removeResponsibility(jobExperienceIndex: number, respIndex: number): void {
    const respArray = this.getResponsibilities(jobExperienceIndex);
    respArray.removeAt(respIndex);
    this.cdRef.detectChanges();
  }

  patchJobExperiences(): void {
    if (!this.resumeJobExperiences) {
      return;
    }

    this.resumeJobExperiences.forEach((experience) => {
      const experienceForm = this.fb.group({
        jobTitle: [experience.jobTitle, Validators.required],
        companyName: [experience.companyName, Validators.required],
        startDate: [experience.startDate, Validators.required],
        endDate: [experience.endDate, Validators.required],
        responsibilities: this.fb.array(
          experience.responsibilities.map((resp) =>
            this.fb.control(resp, Validators.required)
          )
        ),
      });

      this.jobExperiences.push(experienceForm);
    });
  }

  saveJobExperiences() {
    if (!this.jobExperiencesForm.valid) {
      alert('Please fill out experiences correctly.');
      return;
    }

    const { jobExperiences }: { jobExperiences: JobExperience[] } =
      this.jobExperiencesForm.value;

    if (this.resumeJobExperiences && this.resumeId) {
      this.resumesService
        .updateJobExpiriences(this.resumeId, jobExperiences)
        .subscribe({
          error: (err) => console.error(err),
        });
    }
  }
}
