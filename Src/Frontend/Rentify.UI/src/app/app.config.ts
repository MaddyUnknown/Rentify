import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';
import { ENVIRONMENT_CONFIG_SERVICE_TOKEN } from './core/services/tokens/environement-config.token';
import { EnvironmentConfigJsonService } from './core/services/implementations/environement-config/environment-config-json.service';
import { MEDIA_SERVICE_TOKEN } from './core/services/tokens/media';
import { MediaMockService } from './core/services/implementations/media/mock-media.service';
import { PROPERTY_SERVICE_TOKEN } from './core/services/tokens/property.token';
import { PropertyMockService } from './core/services/implementations/property/property-mock.service';
import { MediaApiService } from './core/services/implementations/media/media-api.service';
import { PropertyApiService } from './core/services/implementations/property/property-api.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(),
    { provide: ENVIRONMENT_CONFIG_SERVICE_TOKEN, useClass: EnvironmentConfigJsonService },
    { provide: MEDIA_SERVICE_TOKEN, useClass: MediaApiService },
    { provide: PROPERTY_SERVICE_TOKEN, useClass: PropertyApiService },
  ],
};
