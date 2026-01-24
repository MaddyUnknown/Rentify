import { InjectionToken } from '@angular/core';
import { RouteService } from '../abstractions/route.service';

export const ROUTE_SERVICE_TOKEN = new InjectionToken<RouteService>('ROUTE_SERVICE_TOKEN');
