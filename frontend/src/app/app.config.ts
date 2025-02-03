import { provideHttpClient, withInterceptors } from '@angular/common/http';
import {
  ApplicationConfig,
  importProvidersFrom,
  provideZoneChangeDetection,
} from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { provideRouter } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { routes } from './app.routes';
import { cookieInterceptor } from './core/interceptors/cookie.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([cookieInterceptor])),
    importProvidersFrom(BrowserAnimationsModule),
    importProvidersFrom(CookieService),
  ],
};
