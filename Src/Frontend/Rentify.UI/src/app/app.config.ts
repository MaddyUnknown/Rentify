import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { ENVIRONMENT_CONFIG_SERVICE_TOKEN } from './core/services/tokens/environement-config.token';
import { EnvironmentConfigJsonService } from './core/services/implementations/environement-config/environment-config-json.service';
import { MEDIA_SERVICE_TOKEN } from './core/services/tokens/media.token';
import { MediaMockService } from './core/services/implementations/media/mock-media.service';
import { PROPERTY_SERVICE_TOKEN } from './core/services/tokens/property.token';
import { PropertyMockService } from './core/services/implementations/property/property-mock.service';
import { MediaApiService } from './core/services/implementations/media/media-api.service';
import { PropertyApiService } from './core/services/implementations/property/property-api.service';
import { ROUTE_SERVICE_TOKEN } from './core/services/tokens/route.token';
import { RouteImplementationService } from './core/services/implementations/route/route.implementation.service';
import { TENANT_SERVICE_TOKEN } from './core/services/tokens/tenant.token';
import { TenantApiService } from './core/services/implementations/tenant/tenant-api.service';
import { TenantMockService } from './core/services/implementations/tenant/tenant-mock.service';
import { USER_SERVICE_TOKEN } from './core/services/tokens/user.token';
import { UserMockService } from './core/services/implementations/user/user-mock.service';
import { UserApiService } from './core/services/implementations/user/user-api.service';
import { ApiErrorHandlerInterceptor } from './core/interceptors/api-error-handler.interceptor';
import { ApiHeaderInterceptor } from './core/interceptors/api-header.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),
    { provide: ENVIRONMENT_CONFIG_SERVICE_TOKEN, useClass: EnvironmentConfigJsonService },
    { provide: MEDIA_SERVICE_TOKEN, useClass: MediaApiService },
    { provide: PROPERTY_SERVICE_TOKEN, useClass: PropertyApiService },
    { provide: TENANT_SERVICE_TOKEN, useClass: TenantApiService },
    { provide: USER_SERVICE_TOKEN, useClass: UserApiService },
    { provide: ROUTE_SERVICE_TOKEN, useClass: RouteImplementationService },
    { provide: HTTP_INTERCEPTORS, useClass: ApiHeaderInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: ApiErrorHandlerInterceptor, multi: true },
  ],
};
