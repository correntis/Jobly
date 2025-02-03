import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { UsersService } from './core/services/users.service';
import { AuthService } from './core/services/auth.service';
import { CompaniesService } from './core/services/companies.service';
import AddResumeRequest from './core/requests/resumes/addResumeRequest';
import { ResumesService } from './core/services/resumes.service';
import UpdateResumeRequest from './core/requests/resumes/updateResumeRequest';
import Certification from './core/models/resumes/certification';
import Education from './core/models/resumes/education';
import { CurrenciesService } from './core/services/currencies.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
})
export class AppComponent {
  constructor() {}
}
