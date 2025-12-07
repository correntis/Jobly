import { Route } from '@angular/router';
import { AppendUserIdGuard } from '../core/guards/apppend-user-id.guard';
import { AuthGuard } from '../core/guards/auth.guard';
import { CompanyRoleGuard } from '../core/guards/company-role.guard';
import { UserRoleGuard } from '../core/guards/user-role.guard';
import { CompanyAccountPageComponent } from '../pages/accounts/company-account/company-account-page.component';
import { UserAccountPageComponent } from '../pages/accounts/user-account/user-account-page.component';
import { VacancyAddPageComponent } from '../pages/vacancy-add/vacancy-add-page.component';
import { RedirectPageComponent } from '../shared/pages/redirect/redirect-page.component';

export const accountRoutes: Route[] = [
  {
    path: 'account/user',
    component: RedirectPageComponent,
    canActivate: [AppendUserIdGuard],
  },
  {
    path: 'account/user/:userId',
    component: UserAccountPageComponent,
    canActivate: [AuthGuard, UserRoleGuard],
  },
  {
    path: 'account/company',
    component: RedirectPageComponent,
    canActivate: [AppendUserIdGuard],
  },
  {
    path: 'account/company/:userId',
    component: CompanyAccountPageComponent,
    canActivate: [AuthGuard, CompanyRoleGuard],
  },
  {
    path: 'account/company/:companyId/vacancy',
    component: VacancyAddPageComponent,
    canActivate: [AuthGuard, CompanyRoleGuard],
  },
];
