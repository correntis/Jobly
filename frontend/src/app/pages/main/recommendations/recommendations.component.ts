import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { CompactVacancyComponent } from '../../../features/vacancies/components/compact-vacancy/compact-vacancy.component';
import Vacancy from '../../../core/models/vacancies/vacancy';
import { VacanciesService } from '../../../core/services/vacancies.service';
import { HashedCookieService } from '../../../core/services/hashedCookie.service';
import { ResumesService } from '../../../core/services/resumes.service';
import { EnvParams } from '../../../environments/environment';
import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { MatButton, MatButtonModule } from '@angular/material/button';
import { Router } from '@angular/router';

@Component({
  selector: 'app-recommendations',
  standalone: true,
  imports: [CommonModule, CompactVacancyComponent, MatButtonModule],
  templateUrl: './recommendations.component.html',
})
export class RecommendationsComponent {
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
