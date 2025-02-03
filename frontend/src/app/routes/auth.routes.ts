import { Route } from '@angular/router';
import { LoginPageComponent } from '../pages/auth/login/login-page.component';
import { RegistrationPageComponent } from '../pages/auth/registration/registration-page.component';

export const authRoutes: Route[] = [
  { path: 'registration', component: RegistrationPageComponent },
  { path: 'login', component: LoginPageComponent },
];
