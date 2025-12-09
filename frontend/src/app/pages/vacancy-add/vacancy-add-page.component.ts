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
import ExperienceLevel from '../../core/models/vacancies/experienceLevel';
import Salary from '../../core/models/vacancies/salary';
import AddVacancyDetailsRequest from '../../core/requests/vacancies/addVacancyDetailsRequest';
import { AddVacancyRequest } from '../../core/requests/vacancies/addVacancyRequest';
import HashService from '../../core/services/hash.service';
import { VacanciesService } from '../../core/services/vacancies.service';
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

  constructor(
    private activatedRoute: ActivatedRoute,
    private hashService: HashService,
    private vacanciesService: VacanciesService,
    private fb: FormBuilder,
    private router: Router
  ) {
    this.vacancyForm = this.fb.group({
      title: ['', Validators.required],
      employmentType: ['', Validators.required],
      deadlineAt: ['', Validators.required],
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

  ngOnInit(): void {
    this.loadRouteParams();
  }

  loadRouteParams(): void {
    this.activatedRoute.params.subscribe((params) => {
      this.companyId = this.hashService.decrypt(params['companyId']);
    });
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
              this.goToAccount();
            },
            error: (err) => console.error(err),
          });
        },
        error: (err) => console.error(err),
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
