import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable, of, shareReplay, switchMap, finalize, catchError, throwError } from 'rxjs';
import { AUTH_HEADER, SKIP_ACCESS_TOKEN_REFRESH, SUBSCRIPTION_HEADER } from '../services/tokens/http-context.token';
import { Inject, Injectable } from '@angular/core';
import { USER_SERVICE_TOKEN } from '../services/tokens/user.token';
import { UserService } from '../services/abstractions/user.service';
import { UnauthorizedError } from '../exceptions/unauthorized-error';

@Injectable()
export class ApiHeaderInterceptor implements HttpInterceptor {
  private static readonly ACCESS_TOKEN_REFRESH_BUFFER_MS = 60000;
  private refreshAccessTokenRequest$: Observable<boolean> | null = null;

  constructor(@Inject(USER_SERVICE_TOKEN) private userService: UserService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (req.context.get(AUTH_HEADER) && !req.context.get(SKIP_ACCESS_TOKEN_REFRESH)) {
      return this.ensureValidAccessToken().pipe(
        switchMap(() => next.handle(this.addHeaders(req))),
        catchError((error: HttpErrorResponse) => {
          if (error instanceof UnauthorizedError) this.userService.logout().subscribe();
          return throwError(() => error);
        }),
      );
    }

    return next.handle(this.addHeaders(req)).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error instanceof UnauthorizedError) this.userService.logout().subscribe();
        return throwError(() => error);
      }),
    );
  }

  private addHeaders(req: HttpRequest<any>): HttpRequest<any> {
    const headers: Record<string, string> = {};

    if (req.context.get(AUTH_HEADER) && this.userService.accessToken?.token) {
      headers['Authorization'] = `Bearer ${this.userService.accessToken.token}`;
    }

    if (req.context.get(SUBSCRIPTION_HEADER) && this.userService.userData?.subscriptionReferenceId) {
      headers['X-Subscription-Refence'] = `${this.userService.userData.subscriptionReferenceId}`;
    }

    return Object.keys(headers).length > 0 ? req.clone({ setHeaders: headers }) : req;
  }

  private ensureValidAccessToken(): Observable<boolean> {
    if (!this.shouldRefreshAccessToken()) {
      return of(true);
    }

    if (!this.refreshAccessTokenRequest$) {
      this.refreshAccessTokenRequest$ = this.userService.refreshAccessToken().pipe(
        shareReplay(1),
        finalize(() => {
          this.refreshAccessTokenRequest$ = null;
        }),
      );
    }

    return this.refreshAccessTokenRequest$;
  }

  private shouldRefreshAccessToken(): boolean {
    const accessToken = this.userService.accessToken;

    if (!accessToken?.expiresAt) {
      return false;
    }

    const expiresAt = new Date(accessToken.expiresAt).getTime();

    if (Number.isNaN(expiresAt)) {
      return false;
    }

    return expiresAt - Date.now() <= ApiHeaderInterceptor.ACCESS_TOKEN_REFRESH_BUFFER_MS;
  }
}
