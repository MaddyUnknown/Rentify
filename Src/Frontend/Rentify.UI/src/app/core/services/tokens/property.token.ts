import { InjectionToken } from '@angular/core';
import { PropertyService } from '../abstractions/property.service';

export const PROPERTY_SERVICE_TOKEN = new InjectionToken<PropertyService>('PROPERTY_SERVICE_TOKEN');
