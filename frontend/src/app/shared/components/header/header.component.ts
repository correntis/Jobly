import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { UserRoles } from '../../../core/enums/userRoles';
import HashService from '../../../core/services/hash.service';
import { EnvService } from '../../../environments/environment';
import { NotificationsMenuComponent } from '../notifications-menu/notifications-menu.component';

type Route = {
  path: string;
  name: string;
  condition: boolean;
};

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule, NotificationsMenuComponent],
  templateUrl: './header.component.html',
})
export class HeaderComponent implements OnInit {
  constructor(
    private envService: EnvService,
    private hashService: HashService,
    private router: Router,
    private cdRef: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.cdRef.detectChanges();
  }

  getHeaderLinksRoutes(): Route[] {
    const routes: Route[] = [
      {
        path: 'recommendations',
        name: 'Рекомендации',
        condition: this.isUser(),
      },
      {
        path: this.getApplicationsRouteForRole(UserRoles.User),
        name: 'Мои отклики',
        condition: this.isUser(),
      },
      {
        path: this.getApplicationsRouteForRole(UserRoles.Company),
        name: 'Отклики на вакансии',
        condition: this.isCompany(),
      },
      {
        path: 'account/user',
        name: 'Личный кабинет',
        condition: this.isUser(),
      },
      {
        path: 'account/company',
        name: 'Кабинет компании',
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
    this.redirectTo(route);
  }

  redirectTo(uri: string) {
    this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
      this.router.navigate([uri]);
    });
  }
}
