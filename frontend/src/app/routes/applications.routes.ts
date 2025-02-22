import { Route } from '@angular/router';
import { AuthGuard } from '../core/guards/auth.guard';
import { ApplicationsChatsPageComponent } from '../pages/applications/applications-chats-page.component';

export const applicationsRoutes: Route[] = [
  {
    path: 'applications/:userId/:forRole',
    component: ApplicationsChatsPageComponent,
    canActivate: [AuthGuard],
  },
];
