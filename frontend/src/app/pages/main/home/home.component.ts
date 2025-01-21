import { VacanciesService } from './../../../core/services/vacancies.service';
import { Component } from '@angular/core';
import { FilteredVacanciesComponent } from '../../../features/vacancies/components/filtered-vacancies/filtered-vacancies.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [FilteredVacanciesComponent],
  templateUrl: './home.component.html',
})
export class HomeComponent {}
