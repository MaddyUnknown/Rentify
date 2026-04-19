import { Observable } from 'rxjs';
import { RegisterUser } from '../../models/user/register-user.model';
import { UserProfile } from '../../models/user/user-profile.model';
import { AccessToken } from '../../models/user/access-token.model';

export interface UserService {
  registerUser(user: RegisterUser): Observable<UserProfile>;
  login(email: string, password: string, rememberMe: boolean): Observable<UserProfile>;
  restoreSession(): Observable<boolean>;
  readonly isAuthenticated: boolean;
  readonly accessToken: AccessToken | null;
  readonly userData: UserProfile | null;
  refreshAccessToken(): Observable<boolean>;
  logout(): Observable<boolean>;
}
