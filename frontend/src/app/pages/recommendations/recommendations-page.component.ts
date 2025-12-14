import { CommonModule } from '@angular/common';
import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
import Vacancy from '../../core/models/vacancies/vacancy';
import { ResumesService } from '../../core/services/resumes.service';
import { VacanciesService } from '../../core/services/vacancies.service';
import { EnvService } from '../../environments/environment';
import { CompactVacancyComponent } from '../../shared/components/compact-vacancy/compact-vacancy.component';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { LoaderComponent } from '../../shared/components/loader/loader.component';

@Component({
  selector: 'app-recommendations-page',
  standalone: true,
  imports: [
    CommonModule,
    CompactVacancyComponent,
    MatButtonModule,
    MatIconModule,
    HeaderComponent,
    LoaderComponent,
  ],
  templateUrl: './recommendations-page.component.html',
})
export class RecommendationsPageComponent {
  resumeId?: string;
  vacanciesList?: Vacancy[];

  resumeExists: boolean = false;

  pageNumber: number = 1;
  pageSize: number = 15;

  isFullLoaded: boolean = false;
  isLoading: boolean = false;

  constructor(
    private resumesService: ResumesService,
    private vacanciesService: VacanciesService,
    private envService: EnvService,
    private router: Router
  ) {}

  ngOnInit() {
    const userId = this.envService.getUserId();

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
      const wasFirstLoad = !this.vacanciesList || this.vacanciesList.length === 0;
      if (wasFirstLoad) {
        this.isLoading = true;
      }

      this.vacanciesService
        .getRecomendationsForResume(
          this.resumeId,
          this.pageNumber,
          this.pageSize
        )
        .subscribe({
          next: (vacancies) => {
            this.isLoading = false;
            
            // If returned empty array, no more data available
            if (vacancies.length === 0) {
              this.isFullLoaded = true;
            } else if (!wasFirstLoad && vacancies.length < this.pageSize) {
              // If this is not the first load and we got less than pageSize, it's the last page
              this.isFullLoaded = true;
            } else {
              // Otherwise, there might be more data
              this.isFullLoaded = false;
            }

            if (this.vacanciesList) {
              this.vacanciesList = [...this.vacanciesList, ...vacancies];
            } else {
              this.vacanciesList = vacancies;
            }
            this.pageNumber++;
          },
          error: (err) => {
            this.isLoading = false;
            console.error(err);
          },
        });
    }
  }

  goToAccount() {
    this.router.navigate(['/account/user']);
  }
}
