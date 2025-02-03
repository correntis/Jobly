import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { EnvService } from '../../environments/environment';
import { UserRoles } from '../enums/userRoles';

@Injectable({
  providedIn: 'root',
})
export class CompanyRoleGuard implements CanActivate {
  constructor(private envSevrice: EnvService) {}

  canActivate(): boolean | Promise<boolean> {
    return this.envSevrice.getUserRoles().includes(UserRoles.Company);
  }
}
