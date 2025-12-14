import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { VacanciesFilter } from '../../../../core/models/vacancies/vacanciesFilter';
import Vacancy from '../../../../core/models/vacancies/vacancy';
import { VacanciesService } from '../../../../core/services/vacancies.service';
import { CompactVacancyComponent } from '../../../../shared/components/compact-vacancy/compact-vacancy.component';
import { LoaderComponent } from '../../../../shared/components/loader/loader.component';

@Component({
  selector: 'app-filtered-vacancies',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    FormsModule,
    CompactVacancyComponent,
    LoaderComponent,
  ],
  templateUrl: './filtered-vacancies.component.html',
})
export class FilteredVacanciesComponent {
  vacanciesFilterForm: FormGroup;
  vacanciesFilter: VacanciesFilter = {
    title: null,
    requirements: [],
    skills: [],
    tags: [],
    responsibilities: [],
    benefits: [],
    technologies: [],
    education: [],
    languages: [],
    experience: null,
    salary: null,
    pageSize: 18,
    pageNumber: 1,
  };

  vacanciesList?: Vacancy[];

  isFullLoaded?: boolean = false;
  isLoading: boolean = false;

  constructor(
    private fb: FormBuilder,
    private cdRef: ChangeDetectorRef,
    private vacanciesService: VacanciesService
  ) {
    this.vacanciesFilterForm = this.fb.group({
      title: [''],
      requirements: this.fb.array([]),
      skills: this.fb.array([]),
      tags: this.fb.array([]),
      responsibilities: this.fb.array([]),
      benefits: this.fb.array([]),
      technologies: this.fb.array([]),
      education: this.fb.array([]),
      languages: this.fb.array([]),
      experience: this.fb.group(
        {
          min: [null, [this.negativeValidator]],
          max: [null, [this.negativeValidator]],
        },
        {
          validators: this.experienceRangeValidator,
        }
      ),
      salary: this.fb.group(
        {
          currency: [''],
          min: [null, [this.negativeValidator]],
          max: [null, [this.negativeValidator]],
        },
        {
          validators: [
            this.experienceRangeValidator,
            this.requiredCurrencyIfMinMaxValidator,
          ],
        }
      ),
    });
  }

  get salary(): FormGroup {
    return this.vacanciesFilterForm.get('salary') as FormGroup;
  }

  get experience(): FormGroup {
    return this.vacanciesFilterForm.get('experience') as FormGroup;
  }

  get currency(): FormControl {
    return this.salary.get('currency') as FormControl;
  }

  ngOnInit() {
    this.searchVacancies();
  }

  searchVacancies(): void {
    const isFirstLoad = !this.vacanciesList || this.vacanciesList.length === 0;
    if (isFirstLoad) {
      this.isLoading = true;
    }

    this.vacanciesService.search(this.vacanciesFilter).subscribe({
      next: (vacancies) => {
        this.isLoading = false;
        
        if (vacancies.length === 0) {
          this.isFullLoaded = true;
        } else if (vacancies.length < this.vacanciesFilter.pageSize) {
          this.isFullLoaded = true;
        } else {
          this.isFullLoaded = false;
        }

        if (this.vacanciesList) {
          this.vacanciesList = [...this.vacanciesList, ...vacancies];
        } else {
          this.vacanciesList = vacancies;
        }

        this.vacanciesFilter.pageNumber++;
      },
      error: (err) => {
        this.isLoading = false;
        console.error(err);
      },
    });
  }

  getFormArrayControls(arrayName: string): FormArray {
    return this.vacanciesFilterForm.get(arrayName) as FormArray;
  }

  addToArray(arrayName: string): void {
    const arrayControl = this.getFormArrayControls(arrayName);

    arrayControl.push(this.fb.group({ name: [''] }));

    this.cdRef.detectChanges();
  }

  removeFromArray(arrayName: string, index: number): void {
    const arrayControl = this.getFormArrayControls(arrayName);
    arrayControl.removeAt(index);

    this.cdRef.detectChanges();
  }

  removeFormArray(arrayName: string, index: number): void {
    this.removeFromArray(arrayName, index);
  }

  addLanguage(): void {
    const languagesArray = this.vacanciesFilterForm.get(
      'languages'
    ) as FormArray;
    const languageGroup = this.fb.group({
      name: ['', Validators.required],
      level: ['', Validators.required],
    });
    languagesArray.push(languageGroup);

    this.cdRef.detectChanges();
  }

  applyFilters(): void {
    if (!this.vacanciesFilterForm.valid || !this.isFormArraysValid()) {
      alert('pls fill out form correctly');
    }

    this.vacanciesFilter = {
      ...this.vacanciesFilter,
      ...this.vacanciesFilterForm.value,
    };

    this.getDetailsKeys().map((keyName) => {
      const key = keyName as keyof VacanciesFilter;
      (this.vacanciesFilter[key] as string[]) = this.parseFormArray(
        this.getFormArrayControls(key)
      );
    });

    this.vacanciesFilter.experience = this.experience.value;
    if (
      !this.vacanciesFilter.experience?.min &&
      !this.vacanciesFilter.experience?.max
    ) {
      this.vacanciesFilter.experience = null;
    }

    this.vacanciesFilter.salary = this.salary.value;
    if (
      this.vacanciesFilter.salary?.currency === '' ||
      (!this.vacanciesFilter.salary?.min && !this.vacanciesFilter.salary?.max)
    ) {
      this.vacanciesFilter.salary = null;
    }

    this.vacanciesFilter.languages =
      this.getFormArrayControls('languages').value;

    this.vacanciesFilter.title = this.vacanciesFilterForm.get('title')?.value || null;
    if (this.vacanciesFilter.title === '') {
      this.vacanciesFilter.title = null;
    }

    this.vacanciesFilter.pageNumber = 1;
    this.isLoading = true;

    this.vacanciesService.search(this.vacanciesFilter).subscribe({
      next: (vacancies) => {
        this.isLoading = false;
        this.vacanciesList = vacancies;
        
        if (vacancies.length < this.vacanciesFilter.pageSize) {
          this.isFullLoaded = true;
        } else {
          this.isFullLoaded = false;
        }
        
        this.vacanciesFilter.pageNumber++;
      },
      error: (err) => {
        this.isLoading = false;
        console.error(err);
      },
    });
  }

  parseFormArray(formArray: FormArray) {
    return formArray.value.map((value: { name: string }) => value.name);
  }

  getDetailsKeys(): string[] {
    return [
      'requirements',
      'skills',
      'tags',
      'responsibilities',
      'benefits',
      'technologies',
      'education',
    ];
  }

  isFormArraysValid() {
    const isFormArraysValid = this.getDetailsKeys().every(
      (key) => this.getFormArrayControls(key).valid
    );

    return this.experience.valid && this.salary.valid && isFormArraysValid;
  }

  negativeValidator(control: any) {
    if (control.value !== null && control.value < 0) {
      return { negative: true };
    }

    return null;
  }

  experienceRangeValidator(group: FormGroup): ValidationErrors | null {
    const min = group.get('min')?.value;
    const max = group.get('max')?.value;
    if (min !== null && max !== null && min >= max) {
      return { minGreaterThanMax: true };
    }
    return null;
  }

  requiredCurrencyIfMinMaxValidator(group: FormGroup): ValidationErrors | null {
    const min = group.get('min')?.value;
    const max = group.get('max')?.value;
    const currency = group.get('currency')?.value;

    if (
      (currency === null || currency === '') &&
      (min !== null || max !== null)
    ) {
      return { requiredCurrency: true };
    }
    return null;
  }
}
