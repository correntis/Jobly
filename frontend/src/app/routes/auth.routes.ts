import { Route } from '@angular/router';
import { RegistrationComponent } from '../pages/auth/registration/registration.component';
import { LoginComponent } from '../pages/auth/login/login/login.component';

export const authRoutes: Route[] = [
  { path: 'registration', component: RegistrationComponent },
  { path: 'login', component: LoginComponent },
];
