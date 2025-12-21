import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiConfig } from '../../environments/api.config';
import User from '../models/user';
import RegistrationRequest from '../requests/auth/registrationRequest';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private basePath: string = ApiConfig.auth;

  constructor(private httpClient: HttpClient) {}

  register(registrationRequest: RegistrationRequest): Observable<string> {
    const registerBody: any = {
      firstName: registrationRequest.firstName,
      lastName: registrationRequest.lastName,
      email: registrationRequest.email,
      password: registrationRequest.password,
      rolesNames: registrationRequest.roles,
    };

    if (registrationRequest.isFullRegistration !== undefined) {
      registerBody.isFullRegistration = registrationRequest.isFullRegistration;
    }

    return this.httpClient.post<string>(
      `${this.basePath}/register`,
      registerBody
    );
  }

  login(email: string, password: string): Observable<User> {
    const loginBody = {
      email,
      password,
    };

    return this.httpClient.post<User>(`${this.basePath}/login`, loginBody);
  }

  logout(): Observable<void> {
    return this.httpClient.post<void>(`${this.basePath}/logout`, {});
  }
}
