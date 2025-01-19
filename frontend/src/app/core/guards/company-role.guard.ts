import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { HashedCookieService } from '../services/hashedCookie.service';
import { EnvParams, EnvService } from '../../environments/environment';
import { UserRoles } from '../enums/userRoles';
import { JsonPipe } from '@angular/common';

@Injectable({
  providedIn: 'root',
})
export class CompanyRoleGuard implements CanActivate {
  constructor(private envSevrice: EnvService) {}

  canActivate(): boolean | Promise<boolean> {
    return this.envSevrice.getUserRoles().includes(UserRoles.Company);
  }
}
