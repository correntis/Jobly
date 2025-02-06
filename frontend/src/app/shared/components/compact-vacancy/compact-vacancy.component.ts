import { CommonModule } from '@angular/common';
import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
import { InteractionType } from '../../../core/enums/interactionType';
import Company from '../../../core/models/company';
import Interaction from '../../../core/models/interaction';
import Vacancy from '../../../core/models/vacancies/vacancy';
import { CompaniesService } from '../../../core/services/companies.service';
import HashService from '../../../core/services/hash.service';
import { EnvService } from '../../../environments/environment';
import { InteractionsService } from './../../../core/services/interactions.service';

@Component({
  selector: 'app-compact-vacancy',
  standalone: true,
  imports: [MatCardModule, CommonModule, MatButtonModule, MatIconModule],
  templateUrl: './compact-vacancy.component.html',
})
export class CompactVacancyComponent implements OnInit {
  @Input() vacancy?: Vacancy;

  company?: Company;
  interaction?: Interaction;

  InteractionType = InteractionType;

  constructor(
    private interactionsService: InteractionsService,
    private companiesService: CompaniesService,
    private envService: EnvService,
    private hashService: HashService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadCompany();
    this.loadUserInteraction();
  }

  loadUserInteraction() {
    if (this.vacancy?.id) {
      this.interactionsService
        .getByUserAndVacancy(this.envService.getUserId(), this.vacancy?.id)
        .subscribe({
          next: (intercation) => (this.interaction = intercation),
          error: (err: HttpErrorResponse) => {
            if (err.status === HttpStatusCode.NotFound) {
              this.interaction = undefined;
            }
          },
        });
    }
  }

  loadCompany(): void {
    if (this.vacancy?.companyId) {
      this.companiesService.get(this.vacancy?.companyId).subscribe({
        next: (company: Company) => (this.company = company),
        error: (err: HttpErrorResponse) => console.error(err),
      });
    }
  }

  redirectToVacancyPage(): void {
    if (this.vacancy?.id) {
      const hashedId: string = this.hashService.encrypt(this.vacancy.id);

      this.router.navigate(['/vacancy', hashedId]);
    }
  }
}
