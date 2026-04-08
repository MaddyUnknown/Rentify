import { Observable, of, switchMap, tap, catchError, throwError } from 'rxjs';
import { RegisterUser } from '../../../models/user/register-user.model';
import { UserProfile } from '../../../models/user/user-profile.model';
import { UserService } from '../../abstractions/user.service';
import { HttpClient, HttpContext } from '@angular/common/http';
import { ENVIRONMENT_CONFIG_SERVICE_TOKEN } from '../../tokens/environement-config.token';
import { EnvironmentConfigJsonService } from '../environement-config/environment-config-json.service';
import { Inject, Injectable } from '@angular/core';
import { ResponseWrapper } from '../../../models/response/response-wrapper.model';
import { unwrapReponse } from '../../../utils/unwrap-response.util';
import { AccessToken } from '../../../models/user/access-token.model';
import { AUTH_HEADER, SUBSCRIPTION_HEADER } from '../../tokens/http-context.token';
import { ROUTE_SERVICE_TOKEN } from '../../tokens/route.token';
import { RouteService } from '../../abstractions/route.service';
import { Router } from '@angular/router';

@Injectable()
export class UserApiService implements UserService {
  private authToken: AccessToken | null = null;
  private userProfile: UserProfile | null = null;

  constructor(
    private httpClient: HttpClient,
    private router: Router,
    @Inject(ENVIRONMENT_CONFIG_SERVICE_TOKEN) private environmentConfigService: EnvironmentConfigJsonService,
    @Inject(ROUTE_SERVICE_TOKEN) private routeService: RouteService,
  ) {}

  registerUser(user: RegisterUser): Observable<UserProfile> {
    return this.httpClient
      .post<ResponseWrapper<UserProfile>>(this.environmentConfigService.apiBaseURL + `users`, user)
      .pipe(unwrapReponse());
  }

  login(email: string, password: string): Observable<UserProfile> {
    return this.httpClient
      .post<ResponseWrapper<AccessToken>>(this.environmentConfigService.apiBaseURL + `users/auth`, { email, password })
      .pipe(
        unwrapReponse(),
        tap((token) => {
          this.authToken = token;
          this.userProfile = null;
        }),
        switchMap(() => this.loadUserData()),
        tap((user) => (this.userProfile = user)),
        catchError((error) => {
          this.clearSession();
          return throwError(() => error);
        }),
      );
  }

  private loadUserData(): Observable<UserProfile> {
    if (this.userProfile) {
      return of(this.userProfile);
    } else {
      return this.httpClient
        .get<
          ResponseWrapper<UserProfile>
        >(this.environmentConfigService.apiBaseURL + `users`, { context: new HttpContext().set(AUTH_HEADER, true) })
        .pipe(unwrapReponse());
    }
  }

  get isAuthenticated(): boolean {
    return this.userProfile !== null && this.authToken !== null;
  }

  getAccessToken(): AccessToken | null {
    return this.authToken;
  }

  getUserData(): UserProfile | null {
    return this.userProfile;
  }

  refreshAccessToken(): Observable<void> {
    return this.httpClient
      .post<ResponseWrapper<AccessToken>>(this.environmentConfigService.apiBaseURL + `users/auth/refresh`, {})
      .pipe(
        unwrapReponse(),
        tap((token) => (this.authToken = token)),
        switchMap(() => of()),
        catchError((error) => {
          this.clearSession();
          return throwError(() => error);
        }),
      );
  }

  logout(): Observable<void> {
    this.clearSession();
    this.router.navigate(this.routeService.auth());
    return of();
  }

  private clearSession(): void {
    this.authToken = null;
    this.userProfile = null;
  }
}
