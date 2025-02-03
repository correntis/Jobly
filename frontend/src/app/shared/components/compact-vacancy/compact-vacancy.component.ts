import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { Router } from '@angular/router';
import Company from '../../../core/models/company';
import Vacancy from '../../../core/models/vacancies/vacancy';
import { CompaniesService } from '../../../core/services/companies.service';
import HashService from '../../../core/services/hash.service';

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
    private hashService: HashService,
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

  redirectToVacancyPage() {
    if (this.vacancy?.id) {
      const hashedId = this.hashService.encrypt(this.vacancy.id);

      this.router.navigate(['/vacancy', hashedId]);
    }
  }
}
