import { inject } from '@angular/core';
import { CanActivateChildFn, CanActivateFn, Router, UrlTree } from '@angular/router';
import { RoutesConstants } from '../constants/routes.constants';
import { USER_SERVICE_TOKEN } from '../services/tokens/user.token';

function redirectToAuth(): UrlTree {
  const router = inject(Router);
  return router.createUrlTree([`/${RoutesConstants.Auth}`]);
}

export const authGuard: CanActivateFn = () => {
  const userService = inject(USER_SERVICE_TOKEN);

  return userService.isAuthenticated ? true : redirectToAuth();
};
