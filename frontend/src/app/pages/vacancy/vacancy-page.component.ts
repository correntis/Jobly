import { CommonModule } from '@angular/common';
import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ActivatedRoute, Router } from '@angular/router';
import Company from '../../core/models/company';
import Interaction from '../../core/models/interaction';
import Vacancy from '../../core/models/vacancies/vacancy';
import VacancyDetails from '../../core/models/vacancies/vacancyDetails';
import AddApplicationRequest from '../../core/requests/applications/addApplicationRequest';
import { ApplicationsService } from '../../core/services/applications.service';
import HashService from '../../core/services/hash.service';
import { InteractionsService } from '../../core/services/interactions.service';
import { VacanciesService } from '../../core/services/vacancies.service';
import { EnvService } from '../../environments/environment';
import { InteractionType } from './../../core/enums/interactionType';
import { CompaniesService } from './../../core/services/companies.service';

@Component({
  selector: 'app-vacancy-page',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule],
  templateUrl: './vacancy-page.component.html',
})
export class VacancyPageComponent {
  vacancyId: string = '';
  vacancy?: Vacancy;
  company?: Company;
  interaction?: Interaction;

  InteractionType = InteractionType;

  alreadyApplied: boolean = false;

  constructor(
    private activatedRoutes: ActivatedRoute,
    private router: Router,
    private vacanciesService: VacanciesService,
    private companiesService: CompaniesService,
    private applicationService: ApplicationsService,
    private interactionsService: InteractionsService,
    private hashService: HashService,
    private envService: EnvService
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
        this.loadInteraction(this.envService.getUserId(), vacancy.id);
        this.loadCompany(this.vacancy.companyId);
      },
      error: (err) => {
        console.error(err);
      },
    });
  }

  loadInteraction(userId: string, vacancyId: string): void {
    this.interactionsService.getByUserAndVacancy(userId, vacancyId).subscribe({
      next: (interaction) => {
        this.interaction = interaction;
      },
      error: (err: HttpErrorResponse) => {
        if (err.status === HttpStatusCode.NotFound) {
          this.postInteraction(InteractionType.Click);
          this.loadInteraction(userId, vacancyId);
        }
      },
    });
  }

  loadCompany(companyId: string) {
    this.companiesService.get(companyId).subscribe({
      next: (company) => (this.company = company),
      error: (err) => console.error(err),
    });
  }

  postInteraction(interactionType: number): void {
    if (this.vacancy) {
      this.interactionsService
        .add(this.envService.getUserId(), this.vacancy.id, interactionType)
        .subscribe({
          error: (err) => console.error(err),
        });
    }
  }

  apply() {
    if (this.vacancy) {
      const userId = this.envService.getUserId();

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

  onInteraction(interactionType: InteractionType) {
    if (interactionType === this.interaction?.type) {
      interactionType = InteractionType.Click;
    }

    if (this.vacancy?.id) {
      this.interactionsService
        .add(this.envService.getUserId(), this.vacancy.id, interactionType)
        .subscribe({
          next: () => {
            if (this.interaction) {
              this.interaction.type = interactionType;
            }
          },
          error: (err) => console.error(err),
        });
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
