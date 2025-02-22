import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { EnvService } from '../../environments/environment';
import { UserRoles } from '../enums/userRoles';

@Injectable({
  providedIn: 'root',
})
export class UserRoleGuard implements CanActivate {
  constructor(private envService: EnvService) {}

  canActivate(): boolean | Promise<boolean> {
    return this.envService.getUserRoles().includes(UserRoles.User);
  }
}
