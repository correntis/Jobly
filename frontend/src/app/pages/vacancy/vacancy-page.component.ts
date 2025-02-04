import { CommonModule } from '@angular/common';
import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { ActivatedRoute, Router } from '@angular/router';
import Company from '../../core/models/company';
import Vacancy from '../../core/models/vacancies/vacancy';
import VacancyDetails from '../../core/models/vacancies/vacancyDetails';
import AddApplicationRequest from '../../core/requests/applications/addApplicationRequest';
import { ApplicationsService } from '../../core/services/applications.service';
import HashService from '../../core/services/hash.service';
import { VacanciesService } from '../../core/services/vacancies.service';
import { EnvParams } from '../../environments/environment';
import { CompaniesService } from './../../core/services/companies.service';
import { HashedCookieService } from './../../core/services/hashedCookie.service';

@Component({
  selector: 'app-vacancy-page',
  standalone: true,
  imports: [CommonModule, MatButtonModule],
  templateUrl: './vacancy-page.component.html',
})
export class VacancyPageComponent {
  vacancyId: string = '';
  vacancy?: Vacancy;
  company?: Company;

  alreadyApplied: boolean = false;

  constructor(
    private activatedRoutes: ActivatedRoute,
    private router: Router,
    private vacanciesService: VacanciesService,
    private companiesService: CompaniesService,
    private applicationService: ApplicationsService,
    private hashService: HashService,
    private hashedCookieService: HashedCookieService
  ) {}

  ngOnInit() {
    this.loadParams();
  }

  loadParams(): void {
    this.activatedRoutes.params.subscribe((params) => {
      this.vacancyId = this.hashService.decrypt(params['id']);
      if (this.vacancyId) {
        this.loadVacancy();
      }
    });
  }

  loadVacancy(): void {
    this.vacanciesService.get(this.vacancyId).subscribe({
      next: (vacancy) => {
        this.vacancy = vacancy;
        this.loadCompany(this.vacancy.companyId);
      },
      error: (err) => {
        console.error('Error fetching vacancy:', err);
      },
    });
  }

  loadCompany(companyId: string) {
    this.companiesService.get(companyId).subscribe({
      next: (company) => (this.company = company),
      error: (err) => console.error(err),
    });
  }

  apply() {
    if (this.vacancy) {
      const userId = this.hashedCookieService.get(EnvParams.UserIdCookieName);

      if (userId) {
        const addApplicationRequest: AddApplicationRequest = {
          userId,
          vacancyId: this.vacancy?.id,
        };

        this.applicationService.add(addApplicationRequest).subscribe({
          error: (err: HttpErrorResponse) => {
            if (err.status === HttpStatusCode.Conflict) {
              this.alreadyApplied = true;
            }
          },
        });
      }
    }
  }

  goToCompany(id: string): void {
    const hashedId = this.hashService.encrypt(id);

    this.router.navigate(['company', hashedId]);
  }

  hasDetails(category: string): boolean {
    const key = category as keyof VacancyDetails;

    return !!this.vacancy?.vacancyDetails?.[key];
  }

  getDetails(category: string): string[] {
    var key = category as keyof VacancyDetails;

    if (this.vacancy?.vacancyDetails) {
      return this.vacancy.vacancyDetails[key] as string[];
    }

    return [];
  }

  getDetailsKeys(): string[] {
    return [
      'requirements',
      'tags',
      'responsibilities',
      'benefits',
      'education',
    ];
  }
}
