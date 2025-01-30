import { Injectable } from '@angular/core';
import {
  CanActivate,
  Router,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
} from '@angular/router';
import { AuthorizeService } from './authorize.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class AuthorizeGuard implements CanActivate {
  constructor(private authorizeService: AuthorizeService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    return this.authorizeService.isAuthenticated().pipe(
      map((isAuthenticated: boolean) => {
        if (!isAuthenticated) {
          console.log('Not authenticated. Redirecting to login page...');
          this.router.navigate(['/authentication/login']);
          return false;
        }
        return true; // Allow navigation if authenticated
      })
    );
  }
}
