import { Route } from '@angular/router';
import { HomeComponent } from '../pages/main/home/home.component';
import { AuthGuard } from '../core/guards/auth.guard';
import { FullVacancyComponent } from '../pages/main/full-vacancy/full-vacancy.component';
import { RecommendationsComponent } from '../pages/main/recommendations/recommendations.component';
import { UserApplicationsComponent } from '../pages/applications/user-applications/user-applications.component';
import { FullResumeComponent } from '../pages/main/full-resume/full-resume.component';
import { AppendUserIdGuard } from '../core/guards/apppend-user-id.guard';
import { RedirectComponent } from '../shared/components/redirect/redirect.component';
import { FullCompanyComponent } from '../pages/main/full-company/full-company.component';
import { VacancyFormComponent } from '../pages/vacancies/vacancy-form/vacancy-form.component';
import { CompanyRoleGuard } from '../core/guards/company-role.guard';

export const mainRoutes: Route[] = [
  { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },
  {
    path: 'vacancy/:id',
    component: FullVacancyComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'recommendations',
    component: RecommendationsComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'resume/:userId',
    component: FullResumeComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'company/:companyId',
    component: FullCompanyComponent,
    canActivate: [AuthGuard],
  },
];
