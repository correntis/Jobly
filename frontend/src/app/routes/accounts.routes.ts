import { Route } from '@angular/router';
import { RedirectComponent } from '../shared/components/redirect/redirect.component';
import { AuthGuard } from '../core/guards/auth.guard';
import { UserRoleGuard } from '../core/guards/user-role.guard';
import { AppendUserIdGuard } from '../core/guards/apppend-user-id.guard';
import { UserAccountComponent } from '../pages/accounts/user-account/user-account.component';
import { CompanyAccountComponent } from '../pages/accounts/company-account/company-account.component';
import { CompanyRoleGuard } from '../core/guards/company-role.guard';

export const accountRoutes: Route[] = [
  {
    path: 'account/user',
    component: RedirectComponent,
    canActivate: [AppendUserIdGuard],
  },
  {
    path: 'account/user/:userId',
    component: UserAccountComponent,
    canActivate: [AuthGuard, UserRoleGuard],
  },
  {
    path: 'account/company',
    component: RedirectComponent,
    canActivate: [AppendUserIdGuard],
  },
  {
    path: 'account/company/:userId',
    component: CompanyAccountComponent,
    canActivate: [AuthGuard, UserRoleGuard],
  },
];
