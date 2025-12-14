import { Routes } from '@angular/router';
import { accountRoutes } from './routes/accounts.routes';
import { applicationsRoutes } from './routes/applications.routes';
import { authRoutes } from './routes/auth.routes';
import { mainRoutes } from './routes/main.routes';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  ...mainRoutes,
  ...authRoutes,
  ...accountRoutes,
  ...applicationsRoutes,
];
