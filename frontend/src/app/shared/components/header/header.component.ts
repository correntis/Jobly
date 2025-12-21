import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { UserRoles } from '../../../core/enums/userRoles';
import HashService from '../../../core/services/hash.service';
import { ThemeService } from '../../../core/services/theme.service';
import { EnvService, EnvParams } from '../../../environments/environment';
import { HashedCookieService } from '../../../core/services/hashedCookie.service';
import { AuthService } from '../../../core/services/auth.service';
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
  isUserMenuOpen = false;

  @ViewChild('userMenuButton', { static: false }) userMenuButton?: ElementRef;

  constructor(
    private envService: EnvService,
    private hashService: HashService,
    private router: Router,
    private cdRef: ChangeDetectorRef,
    private themeService: ThemeService,
    private hashedCookieService: HashedCookieService,
    private authService: AuthService
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

  toggleUserMenu(): void {
    this.isUserMenuOpen = !this.isUserMenuOpen;
  }

  logout(): void {
    // Вызываем logout на бэке для очистки cookies
    this.authService.logout().subscribe({
      next: () => {
        // Удаляем cookies на фронте (на всякий случай)
        this.hashedCookieService.delete(EnvParams.UserIdCookieName);
        this.hashedCookieService.delete(EnvParams.UserRoleCookieName);
        
        // Очищаем localStorage от токенов
        if (typeof window !== 'undefined' && window.localStorage) {
          const keysToRemove: string[] = [];
          for (let i = 0; i < localStorage.length; i++) {
            const key = localStorage.key(i);
            if (key && (key.toLowerCase().includes('token') || key.toLowerCase().includes('auth'))) {
              keysToRemove.push(key);
            }
          }
          keysToRemove.forEach(key => localStorage.removeItem(key));
        }
        
        // Закрываем меню
        this.isUserMenuOpen = false;
        
        // Перенаправляем на страницу входа
        this.router.navigate(['/login']);
      },
      error: () => {
        // Даже если запрос не удался, очищаем на фронте и перенаправляем
        this.hashedCookieService.delete(EnvParams.UserIdCookieName);
        this.hashedCookieService.delete(EnvParams.UserRoleCookieName);
        this.isUserMenuOpen = false;
        this.router.navigate(['/login']);
      }
    });
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (this.isUserMenuOpen && this.userMenuButton) {
      const clickedInside = this.userMenuButton.nativeElement.contains(event.target);
      if (!clickedInside) {
        this.isUserMenuOpen = false;
      }
    }
  }
}
