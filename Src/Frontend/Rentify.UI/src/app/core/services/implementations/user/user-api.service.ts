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
import { AUTH_HEADER, SKIP_ACCESS_TOKEN_REFRESH, SUBSCRIPTION_HEADER } from '../../tokens/http-context.token';
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

  login(email: string, password: string, rememberMe: boolean = false): Observable<UserProfile> {
    return this.httpClient
      .post<ResponseWrapper<AccessToken>>(
        this.environmentConfigService.apiBaseURL + `users/auth`,
        { email, password, rememberMe },
        {
          withCredentials: true,
        },
      )
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

  restoreSession(): Observable<boolean> {
    if (this.isAuthenticated) {
      return of(true);
    }

    return this.refreshAccessToken().pipe(
      switchMap(() => of(true)),
      catchError(() => {
        this.clearSession();
        return of(false);
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

  get accessToken(): AccessToken | null {
    return this.authToken;
  }

  get userData(): UserProfile | null {
    return this.userProfile;
  }

  refreshAccessToken(): Observable<boolean> {
    return this.httpClient
      .post<ResponseWrapper<AccessToken>>(
        this.environmentConfigService.apiBaseURL + `users/auth/refresh`,
        {},
        {
          context: new HttpContext().set(SKIP_ACCESS_TOKEN_REFRESH, true),
          withCredentials: true,
        },
      )
      .pipe(
        unwrapReponse(),
        tap((token) => {
          this.authToken = token;
          this.userProfile = null;
        }),
        switchMap(() => this.loadUserData()),
        tap((user) => (this.userProfile = user)),
        switchMap(() => of(true)),
        catchError((error) => {
          this.clearSession();
          return throwError(() => error);
        }),
      );
  }

  logout(): Observable<boolean> {
    return this.httpClient
      .post<ResponseWrapper<any>>(
        this.environmentConfigService.apiBaseURL + `users/auth/logout`,
        {},
        {
          context: new HttpContext().set(SKIP_ACCESS_TOKEN_REFRESH, true),
          withCredentials: true,
        },
      )
      .pipe(
        unwrapReponse(),
        switchMap(() => {
          this.clearSession();
          this.router.navigate(this.routeService.auth());
          return of(true);
        }),
        catchError((error) => {
          this.clearSession();
          return throwError(() => error);
        }),
      );
  }

  /*************  ✨ Windsurf Command ⭐  *************/
  /**
   * Clears the current user session data
   */
  /*******  4bb6c04b-f29a-4705-9408-aa2e3e3d12b1  *******/
  private clearSession(): void {
    this.authToken = null;
    this.userProfile = null;
  }
}
