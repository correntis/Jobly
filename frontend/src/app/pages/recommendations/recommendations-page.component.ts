import { CommonModule } from '@angular/common';
import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { Router } from '@angular/router';
import Vacancy from '../../core/models/vacancies/vacancy';
import { HashedCookieService } from '../../core/services/hashedCookie.service';
import { ResumesService } from '../../core/services/resumes.service';
import { VacanciesService } from '../../core/services/vacancies.service';
import { EnvParams } from '../../environments/environment';
import { CompactVacancyComponent } from '../../shared/components/compact-vacancy/compact-vacancy.component';

@Component({
  selector: 'app-recommendations-page',
  standalone: true,
  imports: [CommonModule, CompactVacancyComponent, MatButtonModule],
  templateUrl: './recommendations-page.component.html',
})
export class RecommendationsPageComponent {
  resumeId?: string;
  vacanciesList?: Vacancy[];

  resumeExists: boolean = false;

  pageNumber: number = 1;
  pageSize: number = 15;

  constructor(
    private resumesService: ResumesService,
    private vacanciesService: VacanciesService,
    private hashedCookieService: HashedCookieService,
    private router: Router
  ) {}

  ngOnInit() {
    const userId = this.hashedCookieService.get(EnvParams.UserIdCookieName);

    if (userId) {
      this.resumesService.getByUser(userId).subscribe({
        next: (resume) => {
          this.resumeId = resume.id;
          this.resumeExists = true;
          this.loadRecommendations();
        },
        error: (err: HttpErrorResponse) => {
          if (err.status === HttpStatusCode.NotFound) {
            this.resumeExists = false;
          }
        },
      });
    }
  }

  loadRecommendations() {
    if (this.resumeId) {
      this.vacanciesService
        .getRecomendationsForResume(
          this.resumeId,
          this.pageNumber,
          this.pageSize
        )
        .subscribe({
          next: (vacancies) => {
            this.vacanciesList = vacancies;
          },
          error: (err) => console.error(err),
        });
    }
  }

  loadMore() {
    if (this.resumeId) {
      this.pageNumber++;

      this.vacanciesService
        .getRecomendationsForResume(
          this.resumeId,
          this.pageNumber,
          this.pageSize
        )
        .subscribe({
          next: (vacancies) => {
            if (this.vacanciesList) {
              this.vacanciesList = [...this.vacanciesList, ...vacancies];
            }
          },
          error: (err) => console.error(err),
        });
    }
  }

  goToAccount() {
    this.router.navigate(['/account/user']);
  }
}
