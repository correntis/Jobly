import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  Input,
  SimpleChange,
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
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { ResumesService } from '../../../../core/services/resumes.service';
import Certification from '../../../../core/models/resumes/certification';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';

@Component({
  selector: 'app-resume-certifications-form',
  standalone: true,
  imports: [
    MatIconModule,
    ReactiveFormsModule,
    CommonModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
  ],
  templateUrl: './resume-certifications-form.component.html',
})
export class ResumeCertificationsFormComponent {
  @Input() resumeCertifications: Certification[] | undefined;
  @Input() resumeId: string | undefined;

  certificationForm: FormGroup;

  constructor(
    private resumesService: ResumesService,
    private fb: FormBuilder,
    private cdRef: ChangeDetectorRef
  ) {
    this.certificationForm = this.fb.group({
      certifications: this.fb.array([]),
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (!this.resumeCertifications) {
      return;
    }

    if (changes['resumeCertifications'] && this.resumeCertifications.length) {
      this.certifications.clear();
      this.patchCertifications();
    }
  }

  get certifications(): FormArray {
    return this.certificationForm.get('certifications') as FormArray;
  }

  lastCertificationIsEmpty(): boolean | undefined {
    var lastControl =
      this.certifications.controls[this.certifications.length - 1];

    if (lastControl) {
      const nameEmpty = lastControl.get('name')?.hasError('required');
      const issuerEmpty = lastControl.get('issuer')?.hasError('required');
      const issueDateEmpty = lastControl.get('issueDate')?.hasError('required');

      return nameEmpty || issuerEmpty || issueDateEmpty;
    }

    return undefined;
  }

  addCertificaion(): void {
    if (this.certifications.length > 0 && this.lastCertificationIsEmpty()) {
      this.certifications.markAllAsTouched();
      return;
    }

    const certificationForm = this.fb.group({
      name: ['', Validators.required],
      issuer: ['', Validators.required],
      issueDate: ['', Validators.required],
    });
    this.certifications.push(certificationForm);
    this.cdRef.detectChanges();
  }

  removeCertification(index: number): void {
    this.certifications.removeAt(index);
    this.cdRef.detectChanges();
  }

  patchCertifications(): void {
    if (!this.resumeCertifications) {
      return;
    }

    this.resumeCertifications.forEach((cert) => {
      this.certifications.push(
        this.fb.group({
          name: [cert.name, Validators.required],
          issuer: [cert.issuer, Validators.required],
          issueDate: [cert.issueDate, Validators.required],
        })
      );
    });
  }

  saveCertifications() {
    if (!this.certificationForm.valid) {
      alert('pls fill out certifications correctly');
      return;
    }

    const { certifications }: { certifications: Certification[] } =
      this.certificationForm.value;

    if (this.resumeCertifications && this.resumeId) {
      this.resumesService
        .updateCertifications(this.resumeId, certifications)
        .subscribe({
          error: (err) => console.error(err),
        });
    }
  }
}
