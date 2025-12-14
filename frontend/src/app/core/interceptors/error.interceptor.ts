import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { EnvParams } from '../../environments/environment';
import { HashedCookieService } from '../services/hashedCookie.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const hashedCookieService = inject(HashedCookieService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        hashedCookieService.delete(EnvParams.UserIdCookieName);
        hashedCookieService.delete(EnvParams.UserRoleCookieName);

        const currentUrl = router.url;
        if (!currentUrl.includes('/login') && !currentUrl.includes('/registration')) {
          router.navigate(['/login']);
        }
      }

      return throwError(() => error);
    })
  );
};

