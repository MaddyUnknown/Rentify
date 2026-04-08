import { HttpEvent, HttpHandler, HttpHeaders, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AUTH_HEADER, SUBSCRIPTION_HEADER } from '../services/tokens/http-context.token';
import { Inject, Injectable } from '@angular/core';
import { USER_SERVICE_TOKEN } from '../services/tokens/user.token';
import { UserService } from '../services/abstractions/user.service';

@Injectable()
export class ApiHeaderInterceptor implements HttpInterceptor {
  constructor(@Inject(USER_SERVICE_TOKEN) private userService: UserService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const header: any = {};

    if (req.context.get(AUTH_HEADER)) {
      header.Authorization = `Bearer ${this.userService.getAccessToken()?.token}`;
    }

    if (req.context.get(SUBSCRIPTION_HEADER)) {
      header['X-Subscription-Refence'] = `${this.userService.getUserData()?.subscriptionReferenceId}`;
    }

    return next.handle(
      req.clone({
        headers: new HttpHeaders(header),
      }),
    );
  }
}
