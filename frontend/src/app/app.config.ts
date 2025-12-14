import { registerLocaleData } from '@angular/common';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import {
  ApplicationConfig,
  importProvidersFrom,
  LOCALE_ID,
  provideZoneChangeDetection,
} from '@angular/core';
import { MAT_DATE_LOCALE } from '@angular/material/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { provideRouter } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import localeRu from '@angular/common/locales/ru';
import { routes } from './app.routes';
import { cookieInterceptor } from './core/interceptors/cookie.interceptor';
import { errorInterceptor } from './core/interceptors/error.interceptor';

registerLocaleData(localeRu);

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([cookieInterceptor, errorInterceptor])),
    importProvidersFrom(BrowserAnimationsModule),
    importProvidersFrom(CookieService),
    { provide: LOCALE_ID, useValue: 'ru' },
    { provide: MAT_DATE_LOCALE, useValue: 'ru' },
  ],
};
