import { Route } from '@angular/router';
import { RedirectComponent } from '../shared/components/redirect/redirect.component';
import { AppendUserIdGuard } from '../core/guards/apppend-user-id.guard';
import { UserApplicationsComponent } from '../pages/applications/user-applications/user-applications.component';
import { UserRoleGuard } from '../core/guards/user-role.guard';
import { AuthGuard } from '../core/guards/auth.guard';

export const applicationsRoutes: Route[] = [
  {
    path: 'applications/:userId/:forRole',
    component: UserApplicationsComponent,
    canActivate: [AuthGuard],
  },
];
