import { Route } from '@angular/router';
import { AuthGuard } from '../core/guards/auth.guard';
import { CompanyPageComponent } from '../pages/company/company-page.component';
import { HomePageComponent } from '../pages/home/home-page.component';
import { RecommendationsPageComponent } from '../pages/recommendations/recommendations-page.component';
import { ResumePageComponent } from '../pages/resume/resume-page.component';
import { VacancyPageComponent } from '../pages/vacancy/vacancy-page.component';

export const mainRoutes: Route[] = [
  { path: 'home', component: HomePageComponent, canActivate: [AuthGuard] },
  {
    path: 'vacancy/:id',
    component: VacancyPageComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'recommendations',
    component: RecommendationsPageComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'resume/:userId',
    component: ResumePageComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'company/:companyId',
    component: CompanyPageComponent,
    canActivate: [AuthGuard],
  },
];
