import { Component } from '@angular/core';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { FilteredVacanciesComponent } from './components/filtered-vacancies/filtered-vacancies.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [FilteredVacanciesComponent, HeaderComponent],
  templateUrl: './home-page.component.html',
})
export class HomePageComponent {}
