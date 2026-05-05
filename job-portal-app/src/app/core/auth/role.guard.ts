import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';
import { UserRole } from './auth.models';

export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isAuthenticated()) {
    return router.createUrlTree(['/login'], {
      queryParams: { returnUrl: state.url },
    });
  }

  const allowedRoles = route.data['roles'] as UserRole[] | undefined;

  if (!allowedRoles?.length) {
    return true;
  }

  const userRole = authService.currentUser()?.role;

  if (userRole && allowedRoles.includes(userRole as UserRole)) {
    return true;
  }

  return router.createUrlTree(['/unauthorized']);
};
