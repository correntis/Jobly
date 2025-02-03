import { Component } from '@angular/core';
import HashService from '../../../core/services/hash.service';
import { ActivatedRoute } from '@angular/router';
import Company from '../../../core/models/company';
import { CompaniesService } from '../../../core/services/companies.service';
import { CommonModule } from '@angular/common';
import Vacancy from '../../../core/models/vacancies/vacancy';
import { CompactVacancyComponent } from '../../../features/vacancies/components/compact-vacancy/compact-vacancy.component';
import { VacanciesService } from '../../../core/services/vacancies.service';

@Component({
  selector: 'app-full-company',
  standalone: true,
  imports: [CommonModule, CompactVacancyComponent],
  templateUrl: './full-company.component.html',
})
export class FullCompanyComponent {
  companyId?: string;

  company?: Company;
  vacanciesList?: Vacancy[];

  constructor(
    private activatedRoutes: ActivatedRoute,
    private hashService: HashService,
    private companiesService: CompaniesService,
    private vacanciesService: VacanciesService
  ) {
    this.activatedRoutes.params.subscribe((params) => {
      this.companyId = this.hashService.decrypt(params['companyId']);
    });
  }

  ngOnInit() {
    this.loadCompany();
    this.loadVacancies();
  }

  loadCompany() {
    if (this.companyId) {
      this.companiesService.get(this.companyId).subscribe({
        next: (company) => {
          this.company = company;
        },
        error: (err) => console.error(err),
      });
    }
  }

  loadVacancies() {
    if (this.companyId) {
      this.vacanciesService.getByCompany(this.companyId).subscribe({
        next: (vacancies) => (this.vacanciesList = vacancies),
        error: (err) => console.error(err),
      });
    }
  }
}
