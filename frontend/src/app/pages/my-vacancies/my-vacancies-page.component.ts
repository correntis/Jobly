import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { Router } from '@angular/router';
import { InteractionType } from '../../core/enums/interactionType';
import HashService from '../../core/services/hash.service';
import { VacanciesService } from '../../core/services/vacancies.service';
import { EnvService } from '../../environments/environment';
import { CompactVacancyComponent } from '../../shared/components/compact-vacancy/compact-vacancy.component';
import { HeaderComponent } from '../../shared/components/header/header.component';
import Vacancy from '../../core/models/vacancies/vacancy';

@Component({
  selector: 'app-my-vacancies-page',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatTabsModule,
    CompactVacancyComponent,
    HeaderComponent,
  ],
  templateUrl: './my-vacancies-page.component.html',
})
export class MyVacanciesPageComponent implements OnInit {
  likedVacancies: Vacancy[] = [];
  dislikedVacancies: Vacancy[] = [];
  viewedVacancies: Vacancy[] = [];

  likedPageNumber: number = 1;
  dislikedPageNumber: number = 1;
  viewedPageNumber: number = 1;
  pageSize: number = 18;

  likedIsFullLoaded: boolean = false;
  dislikedIsFullLoaded: boolean = false;
  viewedIsFullLoaded: boolean = false;

  selectedTab: number = 0;

  constructor(
    private vacanciesService: VacanciesService,
    private envService: EnvService,
    private router: Router,
    private hashService: HashService
  ) {}

  ngOnInit() {
    this.loadLikedVacancies();
  }

  onTabChange(index: number) {
    this.selectedTab = index;
    if (index === 0 && this.likedVacancies.length === 0) {
      this.loadLikedVacancies();
    } else if (index === 1 && this.dislikedVacancies.length === 0) {
      this.loadDislikedVacancies();
    } else if (index === 2 && this.viewedVacancies.length === 0) {
      this.loadViewedVacancies();
    }
  }

  loadLikedVacancies() {
    const userId = this.envService.getUserId();
    if (!userId) return;

    this.vacanciesService
      .getVacanciesByInteraction(userId, InteractionType.Like, this.likedPageNumber, this.pageSize)
      .subscribe({
        next: (vacancies) => {
          if (vacancies.length === 0) {
            this.likedIsFullLoaded = true;
          } else if (vacancies.length < this.pageSize) {
            this.likedIsFullLoaded = true;
          } else {
            this.likedIsFullLoaded = false;
          }

          if (this.likedVacancies.length > 0) {
            this.likedVacancies = [...this.likedVacancies, ...vacancies];
          } else {
            this.likedVacancies = vacancies;
          }

          this.likedPageNumber++;
        },
        error: (err) => console.error(err),
      });
  }

  loadDislikedVacancies() {
    const userId = this.envService.getUserId();
    if (!userId) return;

    this.vacanciesService
      .getVacanciesByInteraction(userId, InteractionType.Dislike, this.dislikedPageNumber, this.pageSize)
      .subscribe({
        next: (vacancies) => {
          if (vacancies.length === 0) {
            this.dislikedIsFullLoaded = true;
          } else if (vacancies.length < this.pageSize) {
            this.dislikedIsFullLoaded = true;
          } else {
            this.dislikedIsFullLoaded = false;
          }

          if (this.dislikedVacancies.length > 0) {
            this.dislikedVacancies = [...this.dislikedVacancies, ...vacancies];
          } else {
            this.dislikedVacancies = vacancies;
          }

          this.dislikedPageNumber++;
        },
        error: (err) => console.error(err),
      });
  }

  loadViewedVacancies() {
    const userId = this.envService.getUserId();
    if (!userId) return;

    this.vacanciesService
      .getVacanciesByInteraction(userId, InteractionType.Click, this.viewedPageNumber, this.pageSize)
      .subscribe({
        next: (vacancies) => {
          if (vacancies.length === 0) {
            this.viewedIsFullLoaded = true;
          } else if (vacancies.length < this.pageSize) {
            this.viewedIsFullLoaded = true;
          } else {
            this.viewedIsFullLoaded = false;
          }

          if (this.viewedVacancies.length > 0) {
            this.viewedVacancies = [...this.viewedVacancies, ...vacancies];
          } else {
            this.viewedVacancies = vacancies;
          }

          this.viewedPageNumber++;
        },
        error: (err) => console.error(err),
      });
  }

  goToVacancy(vacancyId: string) {
    const hashedId = this.hashService.encrypt(vacancyId);
    this.router.navigate(['/vacancy', hashedId]);
  }
}

