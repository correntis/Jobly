import { VacanciesService } from './../../../../core/services/vacancies.service';
import { Component, Input } from '@angular/core';
import { FilteredVacanciesComponent } from '../filtered-vacancies/filtered-vacancies.component';
import Vacancy from '../../../../core/models/vacancies/vacancy';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { CommonModule, DecimalPipe } from '@angular/common';
import Company from '../../../../core/models/company';
import { CompaniesService } from '../../../../core/services/companies.service';
import { MatButton, MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-compact-vacancy',
  standalone: true,
  imports: [MatCardModule, CommonModule, MatButtonModule],
  templateUrl: './compact-vacancy.component.html',
})
export class CompactVacancyComponent {
  @Input() vacancy?: Vacancy;

  company?: Company;

  constructor(
    private companiesService: CompaniesService,
    private router: Router
  ) {}

  ngOnInit() {
    if (this.vacancy?.companyId) {
      this.companiesService.get(this.vacancy?.companyId).subscribe({
        next: (company) => (this.company = company),
        error: (err) => console.error(err),
      });
    }
  }
}
