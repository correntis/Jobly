import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { EnvService } from '../../environments/environment';
import HashService from '../../core/services/hash.service';
import { FilteredVacanciesComponent } from './components/filtered-vacancies/filtered-vacancies.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [FilteredVacanciesComponent, HeaderComponent],
  templateUrl: './home-page.component.html',
})
export class HomePageComponent implements OnInit {
  constructor(
    private router: Router,
    private envService: EnvService,
    private hashService: HashService
  ) {}

  ngOnInit(): void {
    // Если пользователь - компания, перенаправляем на профиль компании
    if (this.envService.isCompany()) {
      const userId = this.envService.getUserId();
      if (userId) {
        const hashedUserId = this.hashService.encrypt(userId);
        this.router.navigate(['/account/company', hashedUserId]);
      }
    }
    // Если пользователь - обычный пользователь, показываем фильтры вакансий (по умолчанию)
  }
}
