import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { EnvService } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(private envService: EnvService, private router: Router) {}

  canActivate(): boolean {
    const userId = this.envService.getUserId();

    if (!userId) {
      this.router.navigate(['/login']);
      return false;
    }

    return true;
  }
}
