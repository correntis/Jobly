import { Routes } from '@angular/router';
import { accountRoutes } from './routes/accounts.routes';
import { authRoutes } from './routes/auth.routes';
import { mainRoutes } from './routes/main.routes';
import { applicationsRoutes } from './routes/applications.routes';

export const routes: Routes = [
  ...mainRoutes,
  ...authRoutes,
  ...accountRoutes,
  ...applicationsRoutes,
];
