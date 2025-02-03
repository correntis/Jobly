import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { UserRoles } from '../../../core/enums/userRoles';
import HashService from '../../../core/services/hash.service';
import { HashedCookieService } from '../../../core/services/hashedCookie.service';
import { EnvParams } from '../../../environments/environment';

type Route = {
  path: string;
  name: string;
  condition: boolean;
};

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './header.component.html',
})
export class HeaderComponent {
  userId?: string;
  userRoles?: string[];

  constructor(
    private hashedCookieService: HashedCookieService,
    private hashService: HashService,
    private rotuer: Router,
    private cdRef: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.userId = this.hashedCookieService.get(EnvParams.UserIdCookieName);

    this.userRoles = JSON.parse(
      this.hashedCookieService.get(EnvParams.UserRoleCookieName)
    );

    this.cdRef.detectChanges();
  }

  getRoutes(): Route[] {
    const routes: Route[] = [
      {
        path: 'recommendations',
        name: 'recommendations',
        condition: this.isUser(),
      },
      {
        path: this.getApplicationsRouteWithRole(UserRoles.User),
        name: 'UserApplications',
        condition: this.isUser(),
      },
      {
        path: this.getApplicationsRouteWithRole(UserRoles.Company),
        name: 'CompanyApplications',
        condition: this.isCompany(),
      },
      {
        path: 'account/user',
        name: 'UserAccount',
        condition: this.isUser(),
      },
      {
        path: 'account/company',
        name: 'CompanyAccount',
        condition: this.isCompany(),
      },
    ];

    return routes;
  }

  getApplicationsRouteWithRole(role: string) {
    let hashedUserId: string = '';
    let hashedForRole: string = '';

    if (this.userId) {
      hashedUserId = this.hashService.encrypt(this.userId);
    }

    hashedForRole = this.hashService.encrypt(role);

    return `applications/${hashedUserId}/${hashedForRole}`;
  }

  isUser(): boolean {
    if (this.userRoles) {
      return this.userRoles?.includes(UserRoles.User);
    }
    return false;
  }

  isCompany(): boolean {
    if (this.userRoles) {
      return this.userRoles?.includes(UserRoles.Company);
    }
    return false;
  }

  goToRoute(route: string) {
    this.rotuer.navigate([route]);
  }
}
