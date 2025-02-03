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
import HashService from '../services/hash.service';

@Injectable({
  providedIn: 'root',
})
export class AppendUserIdGuard implements CanActivate {
  constructor(
    private envSevrice: EnvService,
    private router: Router,
    private hashService: HashService
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean | UrlTree {
    const userId: string = this.envSevrice.getUserId();

    if (userId) {
      if (state.url.indexOf(userId) === -1) {
        var hashedUserId = this.hashService.encrypt(userId);

        const modifiedUrl = `${state.url}/${hashedUserId}`;
        return this.router.createUrlTree([modifiedUrl]);
      }
      return true;
    }

    return this.router.createUrlTree(['/login']);
  }
}
