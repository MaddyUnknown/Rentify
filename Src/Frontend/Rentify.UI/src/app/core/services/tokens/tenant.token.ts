import { InjectionToken } from '@angular/core';
import { TenantService } from '../abstractions/tenant.service';

export const TENANT_SERVICE_TOKEN = new InjectionToken<TenantService>('TENANT_SERVICE_TOKEN');
