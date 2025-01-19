import { Route } from '@angular/router';
import { HomeComponent } from '../pages/main/home/home.component';
import { AuthGuard } from '../core/guards/auth.guard';

export const mainRoutes: Route[] = [
  { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },
];
