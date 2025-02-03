import { VacanciesService } from './../../../core/services/vacancies.service';
import { Component } from '@angular/core';
import { FilteredVacanciesComponent } from '../../../features/vacancies/components/filtered-vacancies/filtered-vacancies.component';
import { HeaderComponent } from '../../../shared/components/header/header.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [FilteredVacanciesComponent, HeaderComponent],
  templateUrl: './home.component.html',
})
export class HomeComponent {}
