import { Injectable } from '@angular/core';
import { UserRoles } from '../core/enums/userRoles';
import { HashedCookieService } from '../core/services/hashedCookie.service';

export class EnvParams {
  public static hashSecretKet = 'secrethashkey';
  public static UserIdCookieName = 'userId';
  public static UserRoleCookieName = 'userRole';

  public static DefaultCurrency = 'USD';

  public static UserIdCookieExpiresDays = 7;
  public static UserRoleCookieExpiresDays = 7;
}

@Injectable({
  providedIn: 'root',
})
export class EnvService {
  constructor(private hashedCookieService: HashedCookieService) {}

  getUserId(): string {
    const userId = this.hashedCookieService.get(EnvParams.UserIdCookieName);
    return userId;
  }

  getUserRoles(): string[] {
    const roles = JSON.parse(
      this.hashedCookieService.get(EnvParams.UserRoleCookieName)
    );
    return roles;
  }

  isUser(): boolean {
    return this.getUserRoles().includes(UserRoles.User);
  }

  isCompany(): boolean {
    return this.getUserRoles().includes(UserRoles.Company);
  }
}
