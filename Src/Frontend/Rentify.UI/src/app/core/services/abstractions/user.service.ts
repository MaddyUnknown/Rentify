import { Observable } from 'rxjs';
import { RegisterUser } from '../../models/user/register-user.model';
import { UserProfile } from '../../models/user/user-profile.model';
import { AccessToken } from '../../models/user/access-token.model';

export interface UserService {
  registerUser(user: RegisterUser): Observable<UserProfile>;
  login(email: string, password: string): Observable<UserProfile>;
  readonly isAuthenticated: boolean;
  getAccessToken(): AccessToken | null;
  getUserData(): UserProfile | null;
  refreshAccessToken(): Observable<void>;
  logout(): Observable<void>;
}
