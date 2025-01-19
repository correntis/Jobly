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
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { ResumesService } from '../../../../core/services/resumes.service';
import Language from '../../../../core/models/resumes/language';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-resume-language-form',
  standalone: true,
  imports: [
    MatIconModule,
    ReactiveFormsModule,
    CommonModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
  ],
  templateUrl: './resume-language-form.component.html',
})
export class ResumeLanguageFormComponent {
  @Input() resumeLanguages: Language[] | undefined = [];
  @Input() resumeId: string | undefined;

  languagesForm: FormGroup;

  constructor(
    private resumesService: ResumesService,
    private fb: FormBuilder,
    private cdRef: ChangeDetectorRef
  ) {
    this.languagesForm = this.fb.group({
      languages: this.fb.array([]),
    });

    this.patchLanguages();
  }

  get languages(): FormArray {
    return this.languagesForm.get('languages') as FormArray;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (!this.resumeLanguages) {
      return;
    }

    if (changes['resumeLanguages'] && this.resumeLanguages.length) {
      this.languages.clear();
      this.patchLanguages();
    }
  }

  patchLanguages(): void {
    if (!this.resumeLanguages) {
      return;
    }

    this.resumeLanguages.forEach((lang) => {
      this.languages.push(
        this.fb.group({
          name: [lang.name, Validators.required],
          level: [lang.level, Validators.required],
        })
      );
    });
  }

  lastLanguageIsEmpty(): boolean | undefined {
    var lastControl = this.languages.controls[this.languages.length - 1];

    if (lastControl) {
      const levelEmpty = lastControl.get('level')?.hasError('required');
      const nameEmpty = lastControl.get('name')?.hasError('required');

      return levelEmpty || nameEmpty;
    }

    return undefined;
  }

  addLanguage(): void {
    if (this.languages.length > 0 && this.lastLanguageIsEmpty()) {
      this.languages.markAllAsTouched();
      return;
    }

    const languageForm = this.fb.group({
      name: ['', Validators.required],
      level: ['', Validators.required],
    });
    this.languages.push(languageForm);
    this.cdRef.detectChanges();
  }

  removeLanguage(index: number): void {
    this.languages.removeAt(index);
    this.cdRef.detectChanges();
  }

  saveLanguages() {
    if (!this.languagesForm.valid) {
      alert('pls fill out languages correctly');
      return;
    }

    const { languages }: { languages: Language[] } = this.languagesForm.value;

    if (this.resumeLanguages && this.resumeId) {
      this.resumesService.updateLanguages(this.resumeId, languages).subscribe({
        next: (id) => console.log('Succesfully updated langs, ', id),
        error: (err) => console.error(err),
      });
    }
  }
}
