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
  title = 'frontend';
  id = '7afec64a-15b9-4736-a9b2-6fc338450f78';
  companyId = 'acc63633-08bd-4907-83e5-a9ee76bcded0';

  constructor(
    private usersService: UsersService,
    private authService: AuthService,
    private companiesService: CompaniesService,
    private resumesService: ResumesService,
    private currenciesService: CurrenciesService
  ) {}

  test() {
    var request: UpdateResumeRequest = {
      id: '67812ae33f284475b2884667',
      userId: '4e9e391d-a903-4302-96f5-2e9bc402e33d',
      title: 'angularar',
      summary: 'angularar',
      skills: ['skill', 'skilereer', 'new'],
      tags: ['tag', 'iri', 'new'],
    };

    var updateCerts: Education[] = [
      {
        institution: 'inst1',
        degree: 'degree1',
        startDate: new Date(Date.now()),
        endDate: new Date(Date.now()),
      },
    ];

    this.currenciesService.get().subscribe({
      next: (curs) => console.log(curs),
      error: (err) => console.error(err),
    });
  }
}
