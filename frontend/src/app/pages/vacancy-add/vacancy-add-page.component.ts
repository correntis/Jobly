import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { ActivatedRoute, Router } from '@angular/router';
import Language from '../../core/models/resumes/language';
import Vacancy from '../../core/models/vacancies/vacancy';
import ExperienceLevel from '../../core/models/vacancies/experienceLevel';
import Salary from '../../core/models/vacancies/salary';
import AddVacancyDetailsRequest from '../../core/requests/vacancies/addVacancyDetailsRequest';
import { AddVacancyRequest } from '../../core/requests/vacancies/addVacancyRequest';
import HashService from '../../core/services/hash.service';
import { VacanciesService } from '../../core/services/vacancies.service';
import { ToastService } from '../../core/services/toast.service';
import { CurrenciesService } from '../../core/services/currencies.service';
import Currency from '../../core/models/currency';
import { HeaderComponent } from '../../shared/components/header/header.component';

@Component({
  selector: 'app-vacancy-add-page',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatButtonModule,
    MatInputModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule,
    HeaderComponent,
  ],
  templateUrl: './vacancy-add-page.component.html',
})
export class VacancyAddPageComponent implements OnInit {
  companyId?: string;

  vacancyForm: FormGroup;
  minDate: string = '';
  companyVacancies: Vacancy[] = [];
  isLoadingVacancies: boolean = false;
  selectedVacancyId: string | null = null;
  currencies: Currency[] = [];

  ngOnInit(): void {
    this.loadRouteParams();
    // Устанавливаем минимальную дату как сегодня
    const today = new Date();
    this.minDate = today.toISOString().split('T')[0];
    this.loadCurrencies();
  }

  loadCurrencies(): void {
    this.currenciesService.get().subscribe({
      next: (currencies) => {
        this.currencies = currencies;
      },
      error: (err) => {
        console.error('Failed to load currencies', err);
      },
    });
  }

  constructor(
    private activatedRoute: ActivatedRoute,
    private hashService: HashService,
    private vacanciesService: VacanciesService,
    private toastService: ToastService,
    private currenciesService: CurrenciesService,
    private fb: FormBuilder,
    private router: Router
  ) {
    this.vacancyForm = this.fb.group({
      title: ['', Validators.required],
      employmentType: ['', Validators.required],
      deadlineAt: ['', [Validators.required, this.futureDateValidator]],
      requirements: this.fb.array([]),
      skills: this.fb.array([]),
      tags: this.fb.array([]),
      responsibilities: this.fb.array([]),
      benefits: this.fb.array([]),
      education: this.fb.array([]),
      technologies: this.fb.array([]),
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
          currency: ['', Validators.required],
          min: [null, [this.negativeValidator]],
          max: [null, [this.negativeValidator]],
        },
        {
          validators: this.experienceRangeValidator,
        }
      ),
    });
  }


  loadRouteParams(): void {
    this.activatedRoute.params.subscribe((params) => {
      this.companyId = this.hashService.decrypt(params['companyId']);
      if (this.companyId) {
        this.loadCompanyVacancies();
      }
    });
  }

  loadCompanyVacancies(): void {
    if (!this.companyId) {
      return;
    }

    this.isLoadingVacancies = true;
    this.vacanciesService.getByCompany(this.companyId).subscribe({
      next: (vacancies) => {
        this.isLoadingVacancies = false;
        this.companyVacancies = vacancies;
      },
      error: (err) => {
        this.isLoadingVacancies = false;
        console.error('Failed to load company vacancies', err);
      },
    });
  }

  onVacancySelect(vacancyId: string): void {
    if (!vacancyId || vacancyId === '') {
      this.selectedVacancyId = null;
      return;
    }

    this.selectedVacancyId = vacancyId;
    
    // Загружаем полную вакансию с деталями
    this.vacanciesService.get(vacancyId).subscribe({
      next: (vacancy) => {
        this.copyVacancyToForm(vacancy);
        this.toastService.success('Данные вакансии скопированы в форму');
      },
      error: (err) => {
        console.error('Failed to load vacancy details', err);
        this.toastService.error('Ошибка при загрузке данных вакансии');
      },
    });
  }

  copyVacancyToForm(vacancy: Vacancy): void {
    // Основные поля
    this.vacancyForm.patchValue({
      title: vacancy.title,
      employmentType: vacancy.employmentType,
      deadlineAt: this.formatDateForInput(vacancy.deadlineAt),
    });

    // Копируем детали вакансии, если они есть
    if (vacancy.vacancyDetails) {
      const details = vacancy.vacancyDetails;

      // Копируем массивы строк
      this.copyStringArrayToFormArray(details.requirements, this.requirements);
      this.copyStringArrayToFormArray(details.skills, this.skills);
      this.copyStringArrayToFormArray(details.tags, this.tags);
      this.copyStringArrayToFormArray(details.responsibilities, this.responsibilities);
      this.copyStringArrayToFormArray(details.benefits, this.benefits);
      this.copyStringArrayToFormArray(details.education, this.education);
      this.copyStringArrayToFormArray(details.technologies, this.technologies);

      // Копируем языки
      if (details.languages && details.languages.length > 0) {
        this.languages.clear();
        details.languages.forEach((lang) => {
          this.languages.push(
            this.fb.group({
              name: [lang.name, Validators.required],
              level: [lang.level, Validators.required],
            })
          );
        });
      }

      // Копируем опыт работы
      if (details.experience) {
        this.experience.patchValue({
          min: details.experience.min,
          max: details.experience.max,
        });
      }

      // Копируем зарплату
      if (details.salary) {
        this.salary.patchValue({
          currency: details.salary.currency,
          min: details.salary.min,
          max: details.salary.max,
        });
      }
    }
  }

  copyStringArrayToFormArray(source: string[], targetFormArray: FormArray): void {
    targetFormArray.clear();
    if (source && source.length > 0) {
      source.forEach((item) => {
        targetFormArray.push(this.fb.control(item));
      });
    }
  }

  formatDateForInput(date: Date | string): string {
    if (!date) {
      return '';
    }
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    return dateObj.toISOString().split('T')[0];
  }

  postVacancy() {
    if (!this.vacancyForm.valid || !this.validateFormArrays()) {
      this.vacancyForm.markAllAsTouched();
      alert('pls fill out form correctly');
    }

    const addVacancyRequest = this.getAddVacancyRequest();

    if (addVacancyRequest) {
      this.vacanciesService.add(addVacancyRequest).subscribe({
        next: (id) => {
          const addVacancyDetailsRequest = this.getAddDetailsRequest(id);
          this.vacanciesService.addDetails(addVacancyDetailsRequest).subscribe({
            next: (id) => {
              this.toastService.success('Вакансия успешно создана');
              this.goToAccount();
            },
            error: (err) => {
              console.error(err);
              this.toastService.error('Ошибка при создании деталей вакансии');
            },
          });
        },
        error: (err) => {
          console.error(err);
          this.toastService.error('Ошибка при создании вакансии');
        },
      });
    }
  }

  getAddDetailsRequest(vacancyId: string): AddVacancyDetailsRequest {
    const requirements = this.parseFormArray(this.requirements);
    const skills = this.parseFormArray(this.skills);
    const tags = this.parseFormArray(this.tags);
    const responsibilities = this.parseFormArray(this.responsibilities);
    const benefits = this.parseFormArray(this.benefits);
    const education = this.parseFormArray(this.education);
    const technologies = this.parseFormArray(this.technologies);
    const languages: Language[] = this.languages.value;
    const experience: ExperienceLevel = this.experience.value;
    const salary: Salary = this.salary.value;

    return {
      vacancyId,
      requirements,
      skills,
      tags,
      responsibilities,
      benefits,
      education,
      technologies,
      languages,
      experience,
      salary,
    };
  }

  getAddVacancyRequest(): AddVacancyRequest | undefined {
    if (!this.companyId) {
      return undefined;
    }

    const { title, employmentType, deadlineAt } = this.vacancyForm.value;

    return {
      title,
      employmentType,
      deadlineAt,
      companyId: this.companyId,
    };
  }

  get requirements(): FormArray {
    return this.vacancyForm.get('requirements') as FormArray;
  }

  get skills(): FormArray {
    return this.vacancyForm.get('skills') as FormArray;
  }

  get tags(): FormArray {
    return this.vacancyForm.get('tags') as FormArray;
  }

  get responsibilities(): FormArray {
    return this.vacancyForm.get('responsibilities') as FormArray;
  }

  get benefits(): FormArray {
    return this.vacancyForm.get('benefits') as FormArray;
  }

  get education(): FormArray {
    return this.vacancyForm.get('education') as FormArray;
  }

  get technologies(): FormArray {
    return this.vacancyForm.get('technologies') as FormArray;
  }

  get languages(): FormArray {
    return this.vacancyForm.get('languages') as FormArray;
  }

  get experience(): FormGroup {
    return this.vacancyForm.get('experience') as FormGroup;
  }

  get salary(): FormGroup {
    return this.vacancyForm.get('salary') as FormGroup;
  }

  parseFormArray(formArray: FormArray) {
    return formArray.value.filter((v: any) => v && (typeof v === 'string' ? v.trim() : v.name?.trim()));
  }

  validateFormArrays() {
    return (
      this.requirements.valid &&
      this.skills.valid &&
      this.tags.valid &&
      this.responsibilities.valid &&
      this.benefits.valid &&
      this.education.valid &&
      this.technologies.valid &&
      this.languages.valid &&
      this.experience.valid &&
      this.salary.valid
    );
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

  futureDateValidator(control: any): ValidationErrors | null {
    if (!control.value) {
      return null; // required validator will handle empty values
    }

    const selectedDate = new Date(control.value);
    const today = new Date();
    today.setHours(0, 0, 0, 0); // Reset time to compare only dates

    if (selectedDate < today) {
      return { pastDate: true };
    }

    return null;
  }

  goToAccount() {
    this.router.navigate(['account/company']);
  }

  // Add/Remove methods for simple arrays
  addRequirement() {
    this.requirements.push(this.fb.control(''));
  }

  removeRequirement(index: number) {
    this.requirements.removeAt(index);
  }

  addSkill() {
    this.skills.push(this.fb.control(''));
  }

  removeSkill(index: number) {
    this.skills.removeAt(index);
  }

  addTechnology() {
    this.technologies.push(this.fb.control(''));
  }

  removeTechnology(index: number) {
    this.technologies.removeAt(index);
  }
}
