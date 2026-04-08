import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { RegisterUser } from '../../../models/user/register-user.model';
import { UserProfile } from '../../../models/user/user-profile.model';
import { UserService } from '../../abstractions/user.service';
import { data } from '../mock/data';
import { AccessToken } from '../../../models/user/access-token.model';
import { RouteService } from '../../abstractions/route.service';
import { ROUTE_SERVICE_TOKEN } from '../../tokens/route.token';
import { Router } from '@angular/router';

@Injectable()
export class UserMockService implements UserService {
  private currentUserId: number | null = null;
  private accessToken = '';
  private refreshToken = '';

  constructor(
    private router: Router,
    @Inject(ROUTE_SERVICE_TOKEN) private routeService: RouteService,
  ) {}

  get isAuthenticated(): boolean {
    return this.currentUserId !== null && this.accessToken.length > 0;
  }

  registerUser(user: RegisterUser): Observable<UserProfile> {
    return new Observable<UserProfile>((observer) => {
      setTimeout(() => {
        const email = user.email.trim().toLowerCase();
        const existingUser = data.users.find((item) => item.email.toLowerCase() === email);

        if (existingUser) {
          observer.error('User already exists');
          return;
        }

        if (user.password !== user.confirmPassword) {
          observer.error('Passwords do not match');
          return;
        }

        const nextId = data.users.reduce((maxId, item) => Math.max(maxId, item.id), 0) + 1;
        const newUser = {
          id: nextId,
          name: user.name.trim(),
          email,
          phoneNumber: user.phoneNumber.trim(),
          password: user.password,
        };

        data.users.push(newUser);

        observer.next({
          id: newUser.id,
          name: user.name,
          email: user.email,
          phoneNumber: user.phoneNumber,
          subscriptionReferenceId: newUser.id + '',
        });

        observer.complete();
      }, data.apiLatency);
    });
  }

  login(email: string, password: string): Observable<UserProfile> {
    return new Observable<UserProfile>((observer) => {
      setTimeout(() => {
        const normalizedEmail = email.trim().toLowerCase();
        const user = data.users.find((item) => item.email.toLowerCase() === normalizedEmail);

        if (!user || user.password !== password) {
          observer.error('Invalid credentials');
          return;
        }

        observer.next(this.startSession(user.id));
        observer.complete();
      }, data.apiLatency);
    });
  }

  getUserData(): UserProfile | null {
    const user = this.getCurrentUser();

    return user
      ? {
          id: user.id,
          name: user.name,
          email: user.email,
          phoneNumber: user.phoneNumber,
          subscriptionReferenceId: user.id + '',
        }
      : null;
  }

  getAccessToken(): AccessToken | null {
    return {
      token: this.accessToken,
      expiresAt: new Date(Date.now() + 3600000),
    };
  }

  refreshAccessToken(): Observable<void> {
    return new Observable<void>((observer) => {
      setTimeout(() => {
        const user = this.getCurrentUser();

        if (!user) {
          observer.error('User is not authenticated');
          return;
        }

        this.accessToken = this.generateToken('access', user.id);
        observer.next();
        observer.complete();
      }, data.apiLatency);
    });
  }

  logout(): Observable<void> {
    return new Observable<void>((observer) => {
      setTimeout(() => {
        this.router.navigate(this.routeService.auth());

        this.currentUserId = null;
        this.accessToken = '';
        this.refreshToken = '';

        observer.next();
        observer.complete();
      }, data.apiLatency);
    });
  }

  private startSession(userId: number): UserProfile {
    this.currentUserId = userId;
    this.accessToken = this.generateToken('access', userId);
    this.refreshToken = this.generateToken('refresh', userId);

    const user = this.getCurrentUser();
    if (!user) {
      throw new Error('Authenticated user not found');
    }

    return {
      id: user.id,
      name: user.name,
      email: user.email,
      phoneNumber: user.phoneNumber,
      subscriptionReferenceId: user.id + '',
    };
  }

  private getCurrentUser() {
    return data.users.find((item) => item.id === this.currentUserId) ?? null;
  }

  private generateToken(tokenType: 'access' | 'refresh', userId: number): string {
    return `mock-${tokenType}-token-${userId}-${Date.now()}`;
  }
}
