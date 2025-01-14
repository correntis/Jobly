import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import HashService from './hash.service';

@Injectable({
  providedIn: 'root',
})
export class HashedCookieService {
  constructor(
    private cookieService: CookieService,
    private hashService: HashService
  ) {}

  set(key: string, value: string, days: number): void {
    const hashedKey = this.hashService.hash(key);
    const encryptedValue = this.hashService.encrypt(value);
    this.cookieService.set(hashedKey, encryptedValue, { expires: 3 });
  }

  get(key: string): string {
    const hashedKey = this.hashService.hash(key);
    const value = this.cookieService.get(hashedKey);
    return this.hashService.decrypt(value);
  }

  delete(key: string): void {
    const hashedKey = this.hashService.hash(key);
    this.cookieService.delete(hashedKey);
  }

  check(key: string) {
    const hashedKey = this.hashService.hash(key);
    this.cookieService.check(hashedKey);
  }
}
