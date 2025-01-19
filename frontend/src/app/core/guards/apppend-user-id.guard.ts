import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  GuardResult,
  MaybeAsync,
  Router,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { EnvService } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AppendUserIdGuard implements CanActivate {
  constructor(private envSevrice: EnvService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean | UrlTree {
    const userId: string = this.envSevrice.getUserId();

    if (userId) {
      if (state.url.indexOf(userId) === -1) {
        const modifiedUrl = `${state.url}/${userId}`;
        return this.router.createUrlTree([modifiedUrl]);
      }
      return true;
    }

    return this.router.createUrlTree(['/login']);
  }
}
