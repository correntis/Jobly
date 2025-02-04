import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { UserRoles } from '../../../core/enums/userRoles';
import HashService from '../../../core/services/hash.service';
import { EnvService } from '../../../environments/environment';

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
export class HeaderComponent implements OnInit {
  constructor(
    private envService: EnvService,
    private hashService: HashService,
    private rotuer: Router,
    private cdRef: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.cdRef.detectChanges();
  }

  getHeaderLinksRoutes(): Route[] {
    const routes: Route[] = [
      {
        path: 'recommendations',
        name: 'recommendations',
        condition: this.isUser(),
      },
      {
        path: this.getApplicationsRouteForRole(UserRoles.User),
        name: 'UserApplications',
        condition: this.isUser(),
      },
      {
        path: this.getApplicationsRouteForRole(UserRoles.Company),
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

  getApplicationsRouteForRole(role: string) {
    let hashedUserId: string = '';
    let hashedForRole: string = '';

    hashedUserId = this.hashService.encrypt(this.envService.getUserId());
    hashedForRole = this.hashService.encrypt(role);

    return `applications/${hashedUserId}/${hashedForRole}`;
  }

  isUser(): boolean {
    return this.envService.isUser();
  }

  isCompany(): boolean {
    return this.envService.isCompany();
  }

  goToRoute(route: string) {
    this.rotuer.navigate([route]);
  }
}
