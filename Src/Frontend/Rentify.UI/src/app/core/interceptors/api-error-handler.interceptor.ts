import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { ApiError } from '../exceptions/api-error';
import { Inject, Injectable } from '@angular/core';
import { USER_SERVICE_TOKEN } from '../services/tokens/user.token';
import { UserService } from '../services/abstractions/user.service';
import { Router } from '@angular/router';
import { RouteService } from '../services/abstractions/route.service';
import { ROUTE_SERVICE_TOKEN } from '../services/tokens/route.token';

@Injectable()
export class ApiErrorHandlerInterceptor implements HttpInterceptor {
  constructor(@Inject(USER_SERVICE_TOKEN) private userService: UserService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) this.userService.logout();

        if (error instanceof ApiError) {
          return throwError(() => error);
        } else if (error?.error?.isSuccess === false) {
          return throwError(() => new ApiError(error?.error?.errorList));
        } else {
          return throwError(() => new ApiError(['Error while processing request']));
        }
      }),
    );
  }
}
