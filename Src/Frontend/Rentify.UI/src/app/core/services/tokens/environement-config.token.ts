import { InjectionToken } from '@angular/core';
import { EnvironmentConfigService } from '../abstractions/environment-config.service';

export const ENVIRONMENT_CONFIG_SERVICE_TOKEN = new InjectionToken<EnvironmentConfigService>(
  'ENVIRONMENT_CONFIG_SERVICE_TOKEN',
);
