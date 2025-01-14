import { Routes } from '@angular/router';
import { RegistrationComponent } from './pages/auth/registration/registration.component';
import { HomeComponent } from './pages/home/home/home.component';
import { LoginComponent } from './pages/auth/login/login/login.component';

export const routes: Routes = [
  { path: 'registration', component: RegistrationComponent },
  { path: 'login', component: LoginComponent },
  { path: 'home', component: HomeComponent },
];
