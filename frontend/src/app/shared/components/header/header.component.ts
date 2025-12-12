import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { UserRoles } from '../../../core/enums/userRoles';
import HashService from '../../../core/services/hash.service';
import { ThemeService } from '../../../core/services/theme.service';
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
  imports: [CommonModule, RouterModule, MatIconModule, NotificationsMenuComponent],
  templateUrl: './header.component.html',
})
export class HeaderComponent implements OnInit {
  isMobileMenuOpen = false;

  constructor(
    private envService: EnvService,
    private hashService: HashService,
    private router: Router,
    private cdRef: ChangeDetectorRef,
    private themeService: ThemeService
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
        path: 'my-vacancies',
        name: 'Мои вакансии',
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

  isActiveRoute(route: string): boolean {
    return this.router.url.includes(route.split('/')[0]);
  }

  goToRoute(route: string) {
    this.redirectTo(route);
  }

  redirectTo(uri: string) {
    this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
      this.router.navigate([uri]);
    });
  }

  toggleTheme(): void {
    this.themeService.toggleTheme();
  }

  isDarkTheme(): boolean {
    return this.themeService.isDark();
  }

  toggleMobileMenu(): void {
    this.isMobileMenuOpen = !this.isMobileMenuOpen;
  }
}
