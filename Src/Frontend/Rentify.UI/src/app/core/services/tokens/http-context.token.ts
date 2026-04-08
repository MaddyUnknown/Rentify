import { HttpContextToken } from '@angular/common/http';

export const AUTH_HEADER = new HttpContextToken<boolean>(() => false);
export const SUBSCRIPTION_HEADER = new HttpContextToken<boolean>(() => false);
